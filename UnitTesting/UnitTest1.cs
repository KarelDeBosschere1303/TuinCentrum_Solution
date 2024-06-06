using Xunit;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Managers;
using TuinCentrumDL_File;
using TuinCentrumDL_SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;

namespace UnitTesting
{
    public class UnitTest1
    {


        private readonly TuinCentrumManager manager;
        private readonly ITuinCentrumRepository repository;

        public UnitTest1()
        {
            //string connectionString = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            //repository = new TuinCentrumRepository(connectionString);
            //var fileProcessor = new FileProcessor(repository);
            //manager = new TuinCentrumManager(fileProcessor, repository);
        }


    }
}