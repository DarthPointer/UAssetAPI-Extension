using System;
using System.Collections.Generic;

namespace UAssetAPI.Extension
{
    public static class UAssetExtension
    {
        public static int SearchForExport(this UAsset asset, FName objectName)
        {
            int currentPos = 0;
            foreach (Export export in asset.Exports)
            {
                currentPos++;
                if (objectName == export.ObjectName) return currentPos;
            }

            return 0;
        }

        public static Import GetImport(this UAsset asset, FName objectName)
        {
            foreach (Import import in asset.Imports)
            {
                if (import.ObjectName == objectName) return import;
            }

            throw new ArgumentException($"The asset does not contain an import called {objectName}");
        }

        public static Export GetExport(this UAsset asset, FName objectName)
        {
            foreach (Export export in asset.Exports)
            {
                if (export.ObjectName == objectName) return export;
            }

            throw new ArgumentException($"The asset does not contain an export called {objectName}");
        }

        public static void AddPreloadDependency(this UAsset asset, IExportReference targetExport, IObjectReference newDependencyObject, PreloadDependency dependencyType)
        {
            Export export = targetExport.GetExport(asset);
            FPackageIndex objectIndex = newDependencyObject.GetObjectIndex(asset);
            int insertIndex = export.FirstExportDependency;

            if (dependencyType == PreloadDependency.SerializationBeforeSerialization)
            {
                asset.PreloadDependencies.Insert(insertIndex, objectIndex);
                export.SerializationBeforeSerializationDependencies++;
            }
            insertIndex += export.SerializationBeforeSerializationDependencies;

            if (dependencyType == PreloadDependency.CreateBeforeSerialization)
            {
                asset.PreloadDependencies.Insert(insertIndex, objectIndex);
                export.CreateBeforeSerializationDependencies++;
            }
            insertIndex += export.CreateBeforeSerializationDependencies;

            if (dependencyType == PreloadDependency.SerializationBeforeCreate)
            {
                asset.PreloadDependencies.Insert(insertIndex, objectIndex);
                export.SerializationBeforeCreateDependencies++;
            }
            insertIndex += export.SerializationBeforeCreateDependencies;

            if (dependencyType == PreloadDependency.CreateBeforeCreate)
            {
                asset.PreloadDependencies.Insert(insertIndex, objectIndex);
                export.CreateBeforeCreateDependencies++;
            }
            //  insertIndex += export.CreateBeforeCreateDependencies;

            bool nextExportIsAffected = false;
            {
                foreach (Export e in asset.Exports)
                {
                    if (nextExportIsAffected)
                    {
                        e.FirstExportDependency++;
                    }

                    if (e == export)
                    {
                        nextExportIsAffected = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="targetExport"></param>
        /// <param name="dependencyObjectToDelete"></param>
        /// <param name="dependencyType"></param>
        /// <returns>Whether a preload dependency was found and deleted.</returns>
        public static bool DeletePreloadDependency(this UAsset asset, IExportReference targetExport, IObjectReference dependencyObjectToDelete, PreloadDependency dependencyType)
        {
            bool wasDeleted = false;

            Export export = targetExport.GetExport(asset);
            FPackageIndex objectIndexToDelete = dependencyObjectToDelete.GetObjectIndex(asset);
            int startIndex, endIndex;

            if (dependencyType == PreloadDependency.SerializationBeforeSerialization)
            {
                startIndex = export.FirstExportDependency;
                endIndex = startIndex + export.SerializationBeforeSerializationDependencies;
            }

            else if (dependencyType == PreloadDependency.CreateBeforeSerialization)
            {
                startIndex = export.FirstExportDependency
                    + export.SerializationBeforeSerializationDependencies;

                endIndex = startIndex + export.CreateBeforeSerializationDependencies;
            }

            else if (dependencyType == PreloadDependency.SerializationBeforeCreate)
            {
                startIndex = export.FirstExportDependency
                    + export.SerializationBeforeSerializationDependencies
                    + export.CreateBeforeSerializationDependencies;

                endIndex = startIndex + export.SerializationBeforeCreateDependencies;
            }

            else // if (dependencyType == PreloadDependency.CreateBeforeCreate)
            {
                startIndex = export.FirstExportDependency
                    + export.SerializationBeforeSerializationDependencies
                    + export.CreateBeforeSerializationDependencies
                    + export.SerializationBeforeCreateDependencies;

                endIndex = startIndex + export.CreateBeforeCreateDependencies;
            }


            int currentIndexIndex = -1;

            foreach (FPackageIndex currentObjectIndex in asset.PreloadDependencies)
            {
                currentIndexIndex++;

                // Keep iterating until we get to preload deps list range.
                if (currentIndexIndex < startIndex)
                {
                    continue;
                }

                // We are done once we leave the desired range. wasDeleted stores if we found and deleted the targeted reference.
                if (currentIndexIndex >= endIndex)
                {
                    break;
                }

                if (currentObjectIndex.Equals(objectIndexToDelete))
                {
                    asset.PreloadDependencies.RemoveAt(currentIndexIndex);

                    wasDeleted = true;
                    break;  // We no longer can iterate through the preload deps list
                }
            }

            // Update preload deps indices if needed.
            if (wasDeleted)
            {
                if (dependencyType == PreloadDependency.SerializationBeforeSerialization)
                {
                    export.SerializationBeforeSerializationDependencies--;
                }
                else if (dependencyType == PreloadDependency.CreateBeforeSerialization)
                {
                    export.CreateBeforeSerializationDependencies--;
                }
                else if (dependencyType == PreloadDependency.SerializationBeforeCreate)
                {
                    export.SerializationBeforeCreateDependencies--;
                }
                else if (dependencyType == PreloadDependency.CreateBeforeCreate)
                {
                    export.CreateBeforeCreateDependencies--;
                }

                bool nextExportIsAffected = false;
                {
                    foreach (Export e in asset.Exports)
                    {
                        if (nextExportIsAffected)
                        {
                            e.FirstExportDependency--;
                        }

                        if (e == export)
                        {
                            nextExportIsAffected = true;
                        }
                    }
                }
            }

            return wasDeleted;
        }
    }
}
