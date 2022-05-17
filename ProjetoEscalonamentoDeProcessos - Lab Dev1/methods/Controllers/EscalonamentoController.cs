using Microsoft.AspNetCore.Mvc;
using methods.Model;

namespace methods.Controllers;

[ApiController]
[Route("/api/escalonamentos/")]

/*
    @description Controller Class for object: "Processo".

    @author Victor Marcelo Ferraz, Paulo, Maxuel.
    @date 2022/17/05
*/

public class EscalonamentoController : ControllerBase
{
  private static List<Processo> listProcesso = new List<Processo>();
  private static int ciclos = 0;  
  private static int countId = 0;
  private int limit = 30;

    /*
        * @description Get all "Processo"
        * @param 
        * @return ActionResult<List<Processo>>
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpGet]
    public ActionResult<List<Processo>> GetAll(){
        if(listProcesso == null || !listProcesso.Any()){
            return StatusCode(200, "Não existem processos na lista");
        }
        List<Processo> getEscalonamentos = listProcesso;
        return Ok(getEscalonamentos.Take(limit));
    }
 
    /*
        * @description Create "Processo"
        * @param [FromBody] Processo? pEscalonamento
        * @return ActionResult<bool>
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpPost("adicionarProcesso")]
    public ActionResult<bool> CreateEscalonamento ([FromBody] Processo? pEscalonamento){
        if(pEscalonamento == null){
            return StatusCode(500, "Objeto inválido.");
        }
        else if(limit > 30){
            return StatusCode(500, "O limite de processos simultaneos foi atingido"); 
        }    
        else if(pEscalonamento.Ciclos < 0){
            return StatusCode(500, "Preencha a quantidade de ciclos");
        }
        else if(pEscalonamento.Nome == null){
            return StatusCode(500, "Seu processo precisa ter nome.");
        }   
        pEscalonamento.Id = countId++;
        pEscalonamento.Status = "Em espera";
        listProcesso.Add(pEscalonamento);

        return Ok("Processo adicionado com sucesso.");
    }  

    /*
        * @description Execute first process 
        * @param 
        * @return ActionResult<bool>
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpPatch("executarProcesso")]
    public ActionResult<bool> executarProcesso(){
        try{Processo processo = listProcesso[0];
         if(processo == null){
            return StatusCode(500, "Lista de execução vazia");
        }
        
        Processo aux = processo;
        aux.Ciclos--;
        listProcesso.Remove(processo);
        try{listProcesso[0].Status = "Em execução";
        }
        catch{}
        try{listProcesso[1].Status = "À executar";
        }
        catch{}
        try{listProcesso[2].Status = "Em espera";
        }
        catch{}
        if(aux.Ciclos > 0){
            aux.Status="Em espera";
            listProcesso.Add(aux);
        } 
        return Ok(listProcesso[0]);
        }

        catch{return StatusCode(500,"Lista vazia");}  
    }   

    /*
        * @description Update ciclos "Processo" by id
        * @param int pId
        * @param int pCiclos
        * @return ActionResult<bool>
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpPatch("atualizarProcesso/{pId}/{pCiclos}")]
    public ActionResult<bool> updateCiclos(int pId, int pCiclos){
        Processo processo = getProcessoByID(pId);
        if(processo == null || processo.Id != pId){
            return StatusCode(404,"Este processo não existe no sistema.");
        }
        if(pCiclos < 0 && processo.Ciclos >= pCiclos){
            return StatusCode(500,"Quantidade de ciclos deve ser maior do que a atual.");
        }
        processo.Ciclos = pCiclos;
        return Ok("Numero de ciclos atualizado com sucesso.");
    }

    /*
        * @description Delete "Processo" by id
        * @param int pId
        * @return ActionResult<bool>

        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpDelete("encerrarProcesso/{pId}")]
    public ActionResult<bool> DeleteProcesso (int pId){
        Processo aux = null;
        Processo processo = getProcessoByID(pId);
        if(processo ==null){
            return NotFound(false);
        }
        else{
            aux = processo;
        }
        if (aux != null && aux.Status != "Em execução"){
            listProcesso.Remove(aux);
            return Ok(true);
        }
        if(aux.Status == "Em execução"){
            return StatusCode(500,"Este processo não pode ser apagado enquanto estiver em execução.");
        
        }
        return NotFound(false);
    }

    /*
        * @description Get processo by id
        * @param id
        * @return Processo 
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpGet("estadoProcesso/{pId}")]
    public ActionResult<bool> getEstadoProcesso(int pId){
        Processo processo = getProcessoByID(pId);

        if(processo == null || processo.Id != pId){
            return StatusCode(404,"Este processo não existe no sistema.");
        }
        return Ok(processo);
    }

    /*
        * @description Get estadoEscalonador
        * @param int id
        * @return Processo 
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    [HttpGet("estadoEscalonador")]
    public ActionResult<bool> getEstadoEscalonador(){
        int escalonadorSize = listProcesso.Count;
        int processoId = listProcesso[0].Id;
        string escalonadorState;

        if(escalonadorSize == 0){
            escalonadorState = "em espera";
        }
        else{
            escalonadorState = "em execução";
        }
        string result = "O tamanho atual da lista é:"+escalonadorSize.ToString()+", o escalonador está "+escalonadorState+" e o  id em processamento é: "+processoId.ToString();


        return Ok(result);
    }

    /*
        * @description Get processo by id
        * @param int id
        * @return Processo 
    
        * @author Victor Marcelo
        * @date 2022-05-17
    */

    public Processo getProcessoByID(int id){
        foreach(Processo processo in listProcesso){
            if(processo.Id == id){
                return processo;
            }
        }
        return null;
    }   
}
