/** @type {import('next').NextConfig} */
const nextConfig = {
    images: {
        remotePatterns: [
            {
                protocol: 'https',
                hostname: '**.pixabay.com/**'
            }
        ]
    }
}

// const nextConfig = {
//     experimental: {
//         serverActions: true
//     },
//     images: {
//         domains: [
//             'cdn.pixabay.com'
//         ]
//     }
// }

module.exports = nextConfig
