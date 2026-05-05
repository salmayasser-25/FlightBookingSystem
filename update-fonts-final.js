const fs = require('fs');
const path = require('path');

const dir = 'e:/projects/airLine-main';
const files = fs.readdirSync(dir).filter(f => f.endsWith('.html'));

const oldLinkPattern = /family=Montserrat:[^&"']+/g;
const newLink = "family=Gelasio:ital,wght@0,400;0,500;0,600;0,700;1,400;1,500;1,600;1,700";

let updatedCount = 0;

files.forEach(file => {
    const filePath = path.join(dir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    let original = content;
    
    // Replace the link part
    content = content.replace(oldLinkPattern, newLink);
    
    // Replace word Montserrat with Gelasio everywhere
    content = content.replace(/Montserrat/g, 'Gelasio');

    if (content !== original) {
        fs.writeFileSync(filePath, content, 'utf8');
        updatedCount++;
    }
});

console.log('Final Font Replace count: ' + updatedCount);
