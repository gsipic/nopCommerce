import {defineConfig, UserConfig} from 'vite'
import fs from 'fs';
import path from 'path';
import react from '@vitejs/plugin-react'
import {spawn} from "node:child_process";

// Get base folder for certificates.
const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ''
        ? `${process.env.APPDATA}/ASP.NET/https`
        : `${process.env.HOME}/.aspnet/https`;

// Generate the certificate name using the NPM package name
const certificateName = process.env.npm_package_name;

// Define certificate filepath
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
// Define key filepath
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

const certDir = path.dirname(certFilePath);

// Ensure the directory exists
if (!fs.existsSync(certDir)) {
    fs.mkdirSync(certDir, { recursive: true });
}
export default defineConfig(async ()=> {
    // Ensure the certificate and key exist
    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        // Wait for the certificate to be generated
        await new Promise<void>((resolve) => {
            spawn('dotnet', [
                'dev-certs',
                'https',
                '--export-path',
                certFilePath,
                '--format',
                'Pem',
                '--no-password',
            ], { stdio: 'inherit', })
                .on('exit', (code) => {
                    resolve();
                    if (code) {
                        process.exit(code);
                    }
                });
        });
    };
    const config: UserConfig = {
        plugins: [react()],
        appType: 'custom',
        root: path.resolve(__dirname),
        
        build: {
            outDir: path.resolve(__dirname, '../wwwroot/dist'), // Output React build to wwwroot
            emptyOutDir: true,
        },
        server: {
            strictPort: true,
            https: {
                cert: certFilePath,
                key: keyFilePath
            }
        },
    }
  return config;
})
