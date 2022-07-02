namespace methods.services;
using methods.Model;


public interface IEscalonamentoService
{
    List<Processo> Get();
    bool Update(int pId, int pCiclos);
    bool Delete(int pId);
    // bool EstadoProcesso(int pId);
    // Processo EstadoProcesso(int pId);
    Processo ProcessoByID(int pId);
    List<Processo> Executar();
    bool Create(Processo? pEscalonamento);
    string getEstadoEscalonador();
}