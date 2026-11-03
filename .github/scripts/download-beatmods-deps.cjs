const { readFile, writeFile, mkdir } = require('node:fs/promises');
const path = require('node:path');
const semver = require('semver');
const { unzipSync } = require('fflate');

const beatModsUrl = 'https://beatmods.com/';

function resolveMods(manifest, catalog) {
  const selected = new Map();
  const approved = catalog.filter(mod => mod.status === 'approved' && mod.gameVersion === manifest.gameVersion);

  function add(mod) {
    const existing = selected.get(mod.name);
    if (existing) {
      if (existing.version !== mod.version) {
        throw new Error(`Conflicting versions of ${mod.name}: ${existing.version} and ${mod.version}`);
      }
      return;
    }
    if (mod.status !== 'approved' || mod.gameVersion !== manifest.gameVersion) {
      throw new Error(`Dependency ${mod.name} ${mod.version} is not approved for ${manifest.gameVersion}`);
    }
    selected.set(mod.name, mod);
    for (const dependency of mod.dependencies ?? []) add(dependency);
  }

  for (const [name, range] of Object.entries(manifest.dependsOn ?? {})) {
    const candidates = approved
      .filter(mod => mod.name === name && semver.valid(mod.version) && semver.satisfies(mod.version, range))
      .sort((a, b) => semver.rcompare(a.version, b.version));
    if (!candidates.length) {
      throw new Error(`No approved ${name} matching '${range}' for Beat Saber ${manifest.gameVersion}`);
    }
    add(candidates[0]);
  }
  return [...selected.values()];
}

function downloadUrl(mod) {
  const download = mod.downloads?.find(item => item.type === 'universal')
    ?? mod.downloads?.find(item => item.type === 'windows');
  if (!download) throw new Error(`No universal or Windows download for ${mod.name} ${mod.version}`);
  // BeatMods has returned both relative paths and absolute URLs.
  const url = new URL(download.url, beatModsUrl);
  if (url.protocol !== 'https:') throw new Error(`Non-HTTPS download URL: ${url}`);
  return url;
}

async function request(url) {
  const response = await fetch(url, { signal: AbortSignal.timeout(120_000) });
  if (!response.ok) throw new Error(`HTTP ${response.status} downloading ${url}`);
  return response;
}

async function extractArchive(archive, output) {
  for (const [name, contents] of Object.entries(unzipSync(archive))) {
    const target = path.resolve(output, name);
    const relative = path.relative(output, target);
    if (relative === '..' || relative.startsWith(`..${path.sep}`) || path.isAbsolute(relative)) {
      throw new Error(`Archive entry escapes destination: ${name}`);
    }
    if (name.endsWith('/')) {
      await mkdir(target, { recursive: true });
    } else {
      await mkdir(path.dirname(target), { recursive: true });
      await writeFile(target, contents);
    }
  }
}

async function main() {
  const [manifestPath, destination] = process.argv.slice(2);
  if (!manifestPath || !destination) throw new Error('Usage: download-beatmods-deps.cjs <manifest.json> <destination>');
  const manifest = JSON.parse((await readFile(manifestPath, 'utf8')).replace(/^\uFEFF/, ''));
  if (!manifest.gameVersion || !manifest.dependsOn) throw new Error('Manifest must contain gameVersion and dependsOn');
  const endpoint = new URL('api/v1/mod', beatModsUrl);
  endpoint.search = new URLSearchParams({ status: 'approved', gameVersion: manifest.gameVersion });
  console.log(`Fetching mods for Beat Saber ${manifest.gameVersion}`);
  const catalog = await (await request(endpoint)).json();
  if (!Array.isArray(catalog)) throw new Error('Unexpected BeatMods response: expected a mod array');
  const mods = resolveMods(manifest, catalog);
  const output = path.resolve(destination);
  await mkdir(output, { recursive: true });
  for (const mod of mods) {
    const url = downloadUrl(mod);
    console.log(`Downloading ${mod.name} ${mod.version}: ${url}`);
    const archive = new Uint8Array(await (await request(url)).arrayBuffer());
    await extractArchive(archive, output);
  }
  console.log(`Downloaded and extracted ${mods.length} mods to ${output}`);
}

module.exports = { resolveMods, downloadUrl, extractArchive };
if (require.main === module) {
  main().catch(error => {
    console.error(error);
    process.exitCode = 1;
  });
}
