const { test } = require('node:test');
const assert = require('node:assert/strict');
const { mkdtemp, readFile, rm } = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { zipSync, strToU8 } = require('fflate');
const { resolveMods, downloadUrl, extractArchive } = require('./download-beatmods-deps.cjs');

function mod(name, version = '1.0.0', dependencies = []) {
  return { name, version, dependencies, gameVersion: '1.44.1', status: 'approved' };
}

test('handles absolute and relative URLs without duplicating the host', () => {
  for (const url of ['/files/BSIPA.zip', 'https://beatmods.com/files/BSIPA.zip']) {
    assert.equal(downloadUrl({ downloads: [{ type: 'universal', url }] }).href,
      'https://beatmods.com/files/BSIPA.zip');
  }
  assert.throws(() => downloadUrl({ downloads: [] }), /No universal or Windows download/);
});

test('selects the highest approved matching version and includes dependencies once', () => {
  const dependency = mod('Shared');
  const newest = mod('Example', '1.2.0', [dependency]);
  const catalog = [mod('Example'), newest, mod('Example', '2.0.0'),
    { ...mod('Example', '1.9.0'), status: 'declined' },
    { ...mod('Example', '1.8.0'), gameVersion: '1.40.7' }, dependency];
  assert.deepEqual(resolveMods({ gameVersion: '1.44.1', dependsOn: { Example: '^1.0.0', Shared: '1.0.0' } }, catalog),
    [newest, dependency]);
});

test('fails on unavailable dependencies and conflicting transitive versions', () => {
  const manifest = { gameVersion: '1.44.1', dependsOn: { Example: '^1.0.0' } };
  assert.throws(() => resolveMods(manifest, []), /No approved Example/);
  const example = mod('Example', '1.0.0', [mod('Shared'), mod('Shared', '2.0.0')]);
  assert.throws(() => resolveMods(manifest, [example]), /Conflicting versions of Shared/);
});

test('extracts plugin paths and rejects archive path traversal', async () => {
  const output = await mkdtemp(path.join(os.tmpdir(), 'beatmods-test-'));
  try {
    await extractArchive(zipSync({ 'Plugins/Example.dll': strToU8('example') }), output);
    assert.equal(await readFile(path.join(output, 'Plugins/Example.dll'), 'utf8'), 'example');
    await assert.rejects(extractArchive(zipSync({ '../escape.dll': strToU8('bad') }), output),
      /Archive entry escapes destination/);
  } finally {
    await rm(output, { recursive: true, force: true });
  }
});
