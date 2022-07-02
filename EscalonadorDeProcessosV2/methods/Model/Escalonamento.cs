namespace methods.Model;

public class Processo{

    
    public int Id {get; set;}
    public String? Status {get; set;}    
    public int Ciclos {get; set;}
    public String Nome {get; set;}
    public Processo(int id, int ciclos, string status, string nome){

        this.Id = id;
        this.Ciclos = ciclos;
        this.Status = status;
        this.Nome = nome;
    }

    

}
