const fs = require('fs');
const path = require('path');

const dir = 'e:/projects/airLine-main';
const files = fs.readdirSync(dir).filter(f => f.endsWith('.html'));

let updatedCount = 0;

files.forEach(file => {
    const filePath = path.join(dir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    let original = content;
    
    // Replace aerofly_logo.svg with globe.png
    content = content.replace(/aerofly_logo\.svg/g, 'globe.png');

    if (content !== original) {
        fs.writeFileSync(filePath, content, 'utf8');
        updatedCount++;
    }
});

console.log('Logo update to globe.png count: ' + updatedCount);
