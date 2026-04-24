const fs = require('fs');
const path = require('path');

const dir = 'e:/projects/airLine-main';
const files = fs.readdirSync(dir).filter(f => f.endsWith('.html'));

const oldFont = `<link href="https://fonts.googleapis.com/css2?family=Inter:opsz,wght@14..32,300;14..32,400;14..32,500;14..32,600;14..32,700;14..32,800&family=Montserrat:wght@400;500;600;700;800;900&display=swap" rel="stylesheet" />`;
const newFont = `<!-- Google Fonts: Gelasio + Inter for professional airline look -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:opsz,wght@14..32,300;14..32,400;14..32,500;14..32,600;14..32,700;14..32,800&family=Gelasio:ital,wght@0,400;0,500;0,600;0,700;1,400;1,500;1,600;1,700&display=swap" rel="stylesheet" />`;

const oldLogoRegex = /<div class="logo-icon">\s*<i class="bi bi-airplane-engines-fill"><\/i>\s*<div class="logo-trail"><\/div>\s*<\/div>/g;
const newLogoIcon = `<div class="logo-icon-custom" style="width: 40px; height: 40px; display: flex; align-items: center; justify-content: center; margin-right: 10px;">
                            <img src="aerofly_logo.svg" alt="AeroFly Logo" style="width: 100%; height: 100%; object-fit: contain; filter: drop-shadow(0 2px 4px rgba(0,0,0,0.2));">
                        </div>`;

const oldFooterRegex = /<div class="footer-logo-icon">\s*<i class="bi bi-airplane-engines-fill"><\/i>\s*<\/div>/g;
const newFooterLogo = `<div class="footer-logo-icon" style="background: transparent;">
                            <img src="aerofly_logo.svg" alt="AeroFly Logo" style="width: 44px; height: 44px; object-fit: contain;">
                        </div>`;

let updatedCount = 0;

files.forEach(file => {
    const filePath = path.join(dir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    let changed = false;
    
    if (content.includes(oldFont)) {
        content = content.replace(oldFont, newFont);
        changed = true;
    }
    
    if (oldLogoRegex.test(content)) {
        content = content.replace(oldLogoRegex, newLogoIcon);
        changed = true;
    }
    
    if (oldFooterRegex.test(content)) {
        content = content.replace(oldFooterRegex, newFooterLogo);
        changed = true;
    }

    if (changed) {
        fs.writeFileSync(filePath, content, 'utf8');
        updatedCount++;
    }
});

console.log('HTML Files updated: ' + updatedCount);

// Update style.css
const cssPath = path.join(dir, 'style.css');
let cssContent = fs.readFileSync(cssPath, 'utf8');
let cssChanged = false;

const cssOldFont1 = `font-family: 'Montserrat', sans-serif;`;
const cssNewFont1 = `font-family: 'Gelasio', serif;`;

if (cssContent.includes(cssOldFont1)) {
    cssContent = cssContent.split(cssOldFont1).join(cssNewFont1);
    cssChanged = true;
}

if (cssChanged) {
    fs.writeFileSync(cssPath, cssContent, 'utf8');
    console.log('style.css updated successfully.');
}
