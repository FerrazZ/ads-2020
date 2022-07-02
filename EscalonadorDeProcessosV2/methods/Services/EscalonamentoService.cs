namespace methods.services;
using methods.Model;
using System.Text.Json;
using System;
using System.Linq;


public class EscalonamentoService : IEscalonamentoService
{
    private static List<Processo> listProcesso = new List<Processo>();
    private static int ciclos = 0;
    private static int countId = 0;
    private int limit = 30;



    public bool Create(Processo? pEscalonamento)
    {

        if (pEscalonamento == null)
        {
            throw new InvalidDataException();
        }
        else if (pEscalonamento.Ciclos < 0)
        {
            throw new NotFiniteNumberException();
        }
        else if (pEscalonamento.Nome == null)
        {
            throw new ArgumentNullException();
        }
        pEscalonamento.Id = countId++;
        pEscalonamento.Status = "Em espera";
        listProcesso.Add(pEscalonamento);

        return true;
    }

    public bool Update(int pId, int pCiclos)
    {

        Processo processo = ProcessoByID(pId);
        if (processo == null || processo.Id != pId)
        {
            throw new NullReferenceException();
        }
        if (pCiclos < 0 && processo.Ciclos >= pCiclos)
        {
            throw new InvalidDataException();
        }

        processo.Ciclos = pCiclos;
        return true;
    }

    public bool Delete(int pId)
    {
        Processo processo = ProcessoByID(pId);
        if (processo == null)
        {
            throw new NullReferenceException();
        }

        if (processo != null && processo.Status != "Em execução")
        {
            listProcesso.Remove(processo);
            return true;
        }
        if (processo.Status == "Em execução")
        {
            throw new InvalidDataException();
        }
        return false;
    }

    public List<Processo> Get()
    {
        if (listProcesso == null || !listProcesso.Any())
        {
            throw new FileNotFoundException();
        }
        List<Processo> getEscalonamentos = listProcesso;
        return listProcesso;
    }

    public Processo EstadoProcesso(int pId)
    {
        Processo processo = ProcessoByID(pId);

        if (processo == null || processo.Id != pId)
        {
            throw new NullReferenceException();
        }
        return processo;
    }

    public List<Processo> Executar()
    {
        
        if (listProcesso.Count < 1){
            throw new NullReferenceException();
        }

        Processo processo = listProcesso[0];
        Processo aux = processo;
        aux.Ciclos--;
        aux.Status = "Em execução";
        listProcesso.Remove(processo);
        
        if (listProcesso.Count>0){
            listProcesso[0].Status = "Em execução";
            aux.Status = "À executar";
        }
        
        if (listProcesso.Count>1){
            listProcesso[1].Status = "À executar";
            aux.Status = "Em espera";
        }

        if (listProcesso.Count>2){
            listProcesso[2].Status = "Em espera";
        }

        if (aux.Ciclos > 0){
            listProcesso.Add(aux);
        }
        else{
            throw new NullReferenceException();
        }
        
        return listProcesso;
    }

    public Processo ProcessoByID(int id)
    {
        foreach (Processo processo in listProcesso)
        {
            if (processo.Id == id)
            {
                return processo;
            }
        }
        return null;
    }

    public string getEstadoEscalonador()
    {
        int escalonadorSize = listProcesso.Count;
        int processoId = listProcesso[0].Id;
        string escalonadorState;

        if (escalonadorSize == 0)
        {
            escalonadorState = "em espera";
        }
        else
        {
            escalonadorState = "em execução";
        }
        return ("O tamanho atual da lista é:" + escalonadorSize.ToString() + ", o escalonador está " + escalonadorState + " e o  id em processamento é: " + processoId.ToString());
    }
}