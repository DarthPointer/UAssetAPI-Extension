using System;

namespace UAssetAPI.Extension
{
    // Not an FName extension since NameData is designed to be initialised out of any UAsset context
    // and has to store an "unresloved" INameReference instead of FString or name index.
    public class NameData : /*FName,*/ IObjectReference
    {
        public INameReference name;
        public int number;

        public NameData(INameReference name, int number = 0)
        {
            this.name = name;
            this.number = number;
        }

        /// <summary>
        /// Generates an FName that exists in the <paramref name="asset"/> and this <see cref="NameData"/> refers.
        /// Throws otherwise.
        /// </summary>
        /// <param name="asset">Context asset that relevant entries are stored in.</param>
        /// <returns></returns>
        public FName AsAssetFName(UAsset asset)
        {
            return new FName(name.GetFString(asset), number);
        }

        FPackageIndex IObjectReference.GetObjectIndex(UAsset asset)
        {
            int importIndex = asset.SearchForImport(this.AsAssetFName(asset));
            if (importIndex < 0) return new FPackageIndex(importIndex);

            int exportIndex = asset.SearchForExport(this.AsAssetFName(asset));
            if (exportIndex > 0) return new FPackageIndex(exportIndex);

            throw new InvalidOperationException($"Name Data refers an object called `{this}` which is not present in the asset");
        }

        Import IObjectReference.GetImport(UAsset asset)
        {
            return asset.GetImport(this.AsAssetFName(asset));
        }

        Export IObjectReference.GetExport(UAsset asset)
        {
            return asset.GetExport(this.AsAssetFName(asset));
        }

        FObjectResource IObjectReference.GetObject(UAsset asset)
        {
            return asset.GetObject(this.AsAssetFName(asset));
        }
    }
}
