using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace FL_Note.Interface
{
    public interface IPhotoLibrary
    {
        void OpenGallery(Action<byte[]> DoAfterCall);

        void CropPhoto(byte[] data, Vector2 Aspect, Vector2 Output, Action<byte[]> DoAfterCall);

        Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);

        bool isApplicationInTheBackground();
    }
}