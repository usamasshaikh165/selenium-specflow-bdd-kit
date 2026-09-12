Run the Vansah BDD import for the feature file currently set in VansahConfig.json.

Usage: /vansah-import
No arguments needed — reads FeatureFileName from VansahConfig.json automatically.

---

Execute the following steps:

## Step 1 — Show current config
Read and display VansahConfig.json so the user can confirm:
- FeatureFileName (the file that will be imported)
- FolderIdentifier (the target Vansah folder)
- ProjectKey

## Step 2 — Run the import
Run this command:

```
dotnet test tools/VansahTools/VansahTools.csproj --filter "Category=VansahImport" --logger "console;verbosity=detailed"
```

## Step 3 — Report results
Parse the output and report:
- How many scenarios were imported successfully
- The SBK-CXXXX test case IDs that were created
- Any failures with their error details
