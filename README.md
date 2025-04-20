# Usage

dotnet DatabaseVisualizer --connectionString "my-connection-string" --includeSchemas "dbo" --excludeSchemas "" --excludeColumns "Active;ModDate;ModName;CreationDate;CreationName" --render "classDiagram;erDiagram[cols:true]" --output "c:\\Temp\\File1.mmd;c:\\Temp\\File2.mmd;c:\\Temp\\File3.mmd"

# Arguments

## connectionString [cs]
## includeSchemas [is]
## excludeSchemas [es]
## excludeColumns [ex]
## render [r]
## output [o]