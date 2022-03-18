using System;

namespace UAssetAPI.Extension
{
    public interface IObjectReference
    {
        FPackageIndex GetObjectIndex(UAsset asset);

        FObjectResource GetObject(UAsset asset);

        Import GetImport(UAsset asset);

        Export GetExport(UAsset asset);
    }

    //public interface IImportReference : IObjectReference
    //{
    //    Import GetImport(UAsset asset);
    //}

    //public interface IExportReference : IObjectReference
    //{
    //    Export GetExport(UAsset asset);
    //}



    public class IndexObjectReference : FPackageIndex, IObjectReference
    {
        public IndexObjectReference(int index) : base(index) { }

        FPackageIndex IObjectReference.GetObjectIndex(UAsset asset)
        {
            if (Index == 0) return new FPackageIndex();

            else if (Index < 0)
            {
                int importListIndex = -(Index + 1);
                if (importListIndex >= asset.Imports.Count)
                {
                    throw new IndexOutOfRangeException
                        ($"Index Object Reference refers import `{Index}` which is not present in the asset");
                }
                // else
                return this;
            }

            else // if (index > 0)
            {
                int exportListIndex = Index - 1;
                if (exportListIndex >= asset.Exports.Count)
                {
                    throw new IndexOutOfRangeException
                        ($"Index Object Reference refers export `{Index}` which is not present in the asset");
                }
                // else
                return this;
            }
        }

        Import IObjectReference.GetImport(UAsset asset)
        {
            return ToImport(asset);
        }

        Export IObjectReference.GetExport(UAsset asset)
        {
            return ToExport(asset);
        }

        FObjectResource IObjectReference.GetObject(UAsset asset)
        {
            if (Index > 0)
            {
                return ToExport(asset);
            }
            if (Index < 0)
            {
                return ToImport(asset);
            }

            return null;
        }
    }
}
