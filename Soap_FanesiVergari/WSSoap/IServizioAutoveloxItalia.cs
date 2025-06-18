using FanesiVergari.Modelli;
using System.ServiceModel;

namespace Soap_FanesiVergari.WSSoap
{
    [ServiceContract(Namespace = "http://soap.fanesivergari.com/")]
    public interface IServizioAutoveloxItalia
    {
        [OperationContract]
        Task<Autovelox[]> GetElencoAutoveloxItalia();

        [OperationContract]
        Task<Autovelox[]> GetElencoAutoveloxPerRegione(string regione);

        [OperationContract]
        Task<string[]> GetElencoRegioniDisponibili();
    }
}