using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PetShop_petro.Classes
{
    public static class Manager
    {
        public static Frame MainFrame { get; set; }

        public static PetModel.User CurrentUser { get; set; }


        public static void GetImagePetModel()
        {
            try
            {
                var list = PetModel.PetrouEntities.GetContext().Product.ToList();

                foreach (var item in list)
                {
                    string path = Directory.GetCurrentDirectory() + @"\img\" + item.ImageName;
                    if (File.Exists(path))
                    {
                        item.Image = File.ReadAllBytes(path);
                    }
                }
                PetModel.PetrouEntities.GetContext().SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}

