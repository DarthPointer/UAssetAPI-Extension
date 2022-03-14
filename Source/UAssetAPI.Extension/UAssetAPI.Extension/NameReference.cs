using System;
using System.Text;

namespace UAssetAPI.Extension
{
    public interface INameReference
    {
        int GetNameIndex(UAsset asset);

        FString GetFString(UAsset asset);
    }

    public class FStringNameReference : FString, INameReference
    {
        public FStringNameReference(string value, Encoding encoding = null) : base(value, encoding) { }

        int INameReference.GetNameIndex(UAsset asset)
        {
            return asset.SearchNameReference(this);
        }

        FString INameReference.GetFString(UAsset asset)
        {
            if (!asset.NameReferenceContains(this))
            {
                throw new InvalidOperationException($"FString Name Reference refers name `{this}` which is not present in the asset");
            }
            // else
            return this;
        }
    }

    public class IndexNameReference : INameReference
    {
        public int index;

        public IndexNameReference(int index)
        {
            this.index = index;
        }

        int INameReference.GetNameIndex(UAsset asset)
        {
            if (index < 0 || index >= asset.GetNameMapIndexList().Count)
            {
                throw new IndexOutOfRangeException
                    ($"Index Name Reference refers index `{index}` which is not present in the asset");
            }

            // else
            return index;
        }

        FString INameReference.GetFString(UAsset asset)
        {
            if (index < 0 || index >= asset.GetNameMapIndexList().Count)
            {
                throw new IndexOutOfRangeException
                    ($"Index Name Reference refers index `{index}` which is not present in the asset");
            }

            // else
            return asset.GetNameReference(index);
        }
    }
}
