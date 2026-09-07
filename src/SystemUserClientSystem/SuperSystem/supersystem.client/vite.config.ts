import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'node:fs';
import path from 'node:path';
import childProcess from 'node:child_process';
import { env } from 'node:process';

export default defineConfig(({ command }) => {
    const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
        env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7236';
    let https;
    // Building static assets does not need to generate or read development certificates.
    if (command === 'serve' && env.SMARTCLOUD_HTTP !== '1') {
        const baseFolder = env.APPDATA ? path.join(env.APPDATA, 'ASP.NET', 'https') : path.join(env.HOME || '', '.aspnet', 'https');
        fs.mkdirSync(baseFolder, { recursive: true });
        const certFilePath = path.join(baseFolder, 'supersystem.client.pem');
        const keyFilePath = path.join(baseFolder, 'supersystem.client.key');
        if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
            if (childProcess.spawnSync('dotnet', ['dev-certs', 'https', '--export-path', certFilePath, '--format', 'Pem', '--no-password'], { stdio: 'inherit' }).status !== 0) {
                throw new Error('Could not create development certificate.');
            }
        }
        https = { key: fs.readFileSync(keyFilePath), cert: fs.readFileSync(certFilePath) };
    }
    return {
        plugins: [plugin()],
        resolve: { alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) } },
        server: {
            proxy: {
                '/api': { target, secure: false },
                '/Redirect': { target, secure: false },
                '/Authenticate': { target, secure: false },
            },
            port: 5173,
            https,
        },
    };
});
