using System;

namespace UAssetAPI.Extension
{
    public class NameData : FName, IObjectReference, IImportReference, IExportReference
    {
        public NameData(INameReference name, UAsset asset, int number = 0) : base(name.GetFString(asset), number) { }

        /// <summary>
        /// Generates an FName that exists in the <paramref name="asset"/> and this <see cref="NameData"/> refers.
        /// Throws otherwise.
        /// </summary>
        /// <param name="asset">Context asset that relevant entries are stored in.</param>
        /// <returns></returns>
        public FName AsAssetFName(UAsset asset)
        {
            return this;
        }

        FPackageIndex IObjectReference.GetObjectIndex(UAsset asset)
        {
            int importIndex = asset.SearchForImport(this);
            if (importIndex < 0) return new FPackageIndex(importIndex);

            int exportIndex = asset.SearchForExport(this);
            if (exportIndex > 0) return new FPackageIndex(exportIndex);

            throw new InvalidOperationException($"Name Data refers an object called `{this}` which is not present in the asset");
        }

        Import IImportReference.GetImport(UAsset asset)
        {
            return asset.GetImport(this);
        }

        Export IExportReference.GetExport(UAsset asset)
        {
            return asset.GetExport(this);
        }
    }
}
