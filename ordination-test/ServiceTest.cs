namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;
using static shared.Util;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    public void OpretDagligSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligSkæve().Count());

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligSkæve().Count());
    }


    [TestMethod]
    public void OpretPN()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(4, service.GetPNs().Count());

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(5, service.GetPNs().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(10, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastLædemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, 8, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastPatientIdNegativ()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(-1,lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastLægemiddelIdNegativ()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, -1, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da lægemiddel id er negativ");
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAtKodenSmiderEnException()
    {
        // Herunder skal man så kalde noget kode,
        // der smider en exception.
        throw new ArgumentNullException("Der er sket en fejl");

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }
}