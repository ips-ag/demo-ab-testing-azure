const fs = require("fs-extra");
const path = require("path");

// Specify the path to the environment file you want to update
const targetPath = path.join(__dirname, "../src/environments/environment.ts");
const backupPath = path.join(
  __dirname,
  "../src/environments/environment.bk.ts"
);
fs.copySync(targetPath, backupPath);

// Read the content of the environment file
let envContent = fs.readFileSync(targetPath, "utf8");

// Replace the placeholder values with actual environment variables
envContent = envContent.replace(
  /process.env.ANALYTICS_CLARITY_ID/g,
  `'${process.env.ANALYTICS_CLARITY_ID || "defaultClarityId"}'`
);
envContent = envContent.replace(
  /process.env.ANALYTICS_GA_MEASUREMENT_ID/g,
  `'${process.env.ANALYTICS_GA_MEASUREMENT_ID || "defaultGAId"}'`
);

// Write the updated content back to the environment file
fs.writeFileSync(targetPath, envContent, "utf8");

console.log("Environment variables have been updated.");
