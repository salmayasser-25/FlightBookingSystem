const fs = require('fs');
const path = require('path');

const dir = 'e:/projects/airLine-main';
const files = fs.readdirSync(dir).filter(f => f.endsWith('.html'));

let updatedCount = 0;

files.forEach(file => {
    const filePath = path.join(dir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    let original = content;
    
    // Replace the specific bootstrap icon used for the logo with globe.png
    // Header/Sidebar cases
    content = content.replace(/<i class="bi bi-airplane-engines-fill"><\/i>\s*AeroFly/g, 
        '<img src="globe.png" alt="Logo" style="width: 32px; height: 32px; object-fit: contain; margin-right: 10px;"> AeroFly');
    
    // Any remaining instances of the icon that might be part of a logo
    // (Avoiding icons that might be used for other purposes, though bi-airplane-engines-fill is quite specific to the logo here)
    content = content.replace(/<div class="logo-icon">\s*<i class="bi bi-airplane-engines-fill"><\/i>\s*<div class="logo-trail"><\/div>\s*<\/div>/g,
        '<div class="logo-icon-custom" style="width: 40px; height: 40px; display: flex; align-items: center; justify-content: center; margin-right: 10px;"><img src="globe.png" alt="AeroFly Logo" style="width: 100%; height: 100%; object-fit: contain; filter: drop-shadow(0 2px 4px rgba(0,0,0,0.2));"></div>');

    content = content.replace(/<div class="footer-logo-icon">\s*<i class="bi bi-airplane-engines-fill"><\/i>\s*<\/div>/g,
        '<div class="footer-logo-icon" style="background: transparent;"><img src="globe.png" alt="AeroFly Logo" style="width: 44px; height: 44px; object-fit: contain;"></div>');

    if (content !== original) {
        fs.writeFileSync(filePath, content, 'utf8');
        updatedCount++;
    }
});

console.log('Final logo cleanup count: ' + updatedCount);
