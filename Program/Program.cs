using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Managers;
using TuinCentrumDL_File;
using TuinCentrumDL_SQL;

namespace Program
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionstring = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            ITuinCentrumRepository tuinCentrumRepository = new TuinCentrumRepository(connectionstring);
            IFileProcessor fileProcessor = new FileProcessor(tuinCentrumRepository);
            TuinCentrumManager tuinCentrumManager = new TuinCentrumManager(fileProcessor, tuinCentrumRepository);
            string filepath = @"C:\Users\karel\Desktop\Ho Gent\Programmeren\EINDOPDRACHT!\tuin\klanten.txt";
            //tuinCentrumManager.SchrijfKlanten(filepath);
            filepath = @"C:\Users\karel\Desktop\Ho Gent\Programmeren\EINDOPDRACHT!\tuin\producten.txt";
            //tuinCentrumManager.SchrijfProducten(filepath);
            filepath = @"C:\Users\karel\Desktop\Ho Gent\Programmeren\EINDOPDRACHT!\tuin\offertes.txt";
            string filepath2 = @"C:\Users\karel\Desktop\Ho Gent\Programmeren\EINDOPDRACHT!\tuin\offerte_producten.txt";
            tuinCentrumManager.SchrijfOffertes(filepath, filepath2);

            Console.WriteLine("Connection string valid");

        }
    }
}
