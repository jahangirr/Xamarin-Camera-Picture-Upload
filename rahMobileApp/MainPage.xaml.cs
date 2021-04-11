using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace rahMobileApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public class EmpLists
        {
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
        }
        public MainPage()
        {
            InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;

            LoadData();
        }

      
        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() {  });
            

            if (photo != null)
            {
                try
                {

                    await UploadBitmapAsync(photo.GetStream());

                    PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                }
                catch (Exception ex)
                {
                    //debug
                    System.Diagnostics.Debug.WriteLine("Exception Caught: " + ex.ToString());

                    return;
                }

            }

           

        }


       

        private const string UPLOAD_IMAGE = "http://www.jahidb.somee.com/imageapi/api/upload/imagePost";
        // Change the above path as per your need
        public async Task<String> UploadBitmapAsync(Stream s)
        {
            byte[] bitmapData;
            using (var streamReader = new MemoryStream())
            {
                s.CopyTo(streamReader);
                bitmapData = streamReader.ToArray();
            }
            var fileContent = new ByteArrayContent(bitmapData);

            Random random = new Random();

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = "my_uploaded_image_"+ random.Next(100,10000).ToString()+ ".jpg"
            };

            string boundary = "---8d0f01e6b3b5dafaaadaad";
            MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);
            multipartContent.Add(fileContent);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(UPLOAD_IMAGE, multipartContent);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return null;
        }




        public async void LoadData()
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://www.jahidb.somee.com/imageapi/api/EmpLists";
            // Change the above path as per your need
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);
            content = await response.Content.ReadAsStringAsync();
            var Items = JsonConvert.DeserializeObject<List<EmpLists>>(content);
            ListView1.ItemsSource = Items;
        }
    }
}
