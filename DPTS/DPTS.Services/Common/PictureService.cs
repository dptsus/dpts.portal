using DPTS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Core;
using DPTS.Domain.Entities;

namespace DPTS.Services.Common
{
    public class PictureService : IPictureService
    {
        public bool StoreInDb
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void DeletePicture(Picture picture)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultPictureUrl(int targetSize = 0, PictureType defaultPictureType = PictureType.Entity, string storeLocation = null)
        {
            throw new NotImplementedException();
        }

        public Picture GetPictureById(int pictureId)
        {
            throw new NotImplementedException();
        }

        public IPagedList<Picture> GetPictures(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IList<Picture> GetPicturesByProductId(int productId, int recordsToReturn = 0)
        {
            throw new NotImplementedException();
        }

        public IDictionary<int, string> GetPicturesHash(int[] picturesIds)
        {
            throw new NotImplementedException();
        }

        public string GetPictureUrl(Picture picture, int targetSize = 0, bool showDefaultPicture = true, PictureType defaultPictureType = PictureType.Entity)
        {
            throw new NotImplementedException();
        }

        public string GetPictureUrl(int pictureId, int targetSize = 0, bool showDefaultPicture = true, PictureType defaultPictureType = PictureType.Entity)
        {
            throw new NotImplementedException();
        }

        public string GetThumbLocalPath(Picture picture, int targetSize = 0, bool showDefaultPicture = true)
        {
            throw new NotImplementedException();
        }

        public Picture InsertPicture(byte[] pictureBinary, string mimeType, bool isNew = true, bool validateBinary = true)
        {
            throw new NotImplementedException();
        }

        public byte[] LoadPictureBinary(Picture picture)
        {
            throw new NotImplementedException();
        }

        public Picture UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType, bool isNew = true, bool validateBinary = true)
        {
            throw new NotImplementedException();
        }

        public byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            throw new NotImplementedException();
        }
    }
}
