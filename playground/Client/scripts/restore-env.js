const fs = require("fs-extra");
const path = require("path");

// Specify the path to the environment file you want to update
const targetPath = path.join(__dirname, "../src/environments/environment.ts");
const backupPath = path.join(
  __dirname,
  "../src/environments/environment.bk.ts"
);
fs.removeSync(targetPath);
fs.copySync(backupPath, targetPath);
