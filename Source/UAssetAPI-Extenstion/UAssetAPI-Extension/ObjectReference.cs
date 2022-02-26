using System;

namespace UAssetAPI.Extension
{
    public interface IObjectReference
    {
        FPackageIndex GetObjectIndex(UAsset asset);
    }

    public interface IImportReference
    {
        Import GetImport(UAsset asset);
    }

    public interface IExportReference
    {
        Export GetExport(UAsset asset);
    }



    public class IndexObjectReference : FPackageIndex, IObjectReference, IImportReference, IExportReference
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

        Import IImportReference.GetImport(UAsset asset)
        {
            return ToImport(asset);
        }

        Export IExportReference.GetExport(UAsset asset)
        {
            return ToExport(asset);
        }
    }
}
