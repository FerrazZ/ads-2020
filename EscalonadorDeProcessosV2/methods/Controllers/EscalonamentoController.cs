namespace methods.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using methods.Model;
using methods.services;

    /*
        * @description API com objetivo de simular um escalonador de processos, usando nome, quantidade de ciclo o posição na fila para ser executado.
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */
    
[ApiController]
[Route("/api/escalonamentos/")]

public class EscalonamentoController : ControllerBase
{
    private readonly IEscalonamentoService _escalonamentoService;

    public EscalonamentoController(IEscalonamentoService escalonamentoService)
    {
        _escalonamentoService = escalonamentoService;
    }

   /*
        * @description Retorna todos processos listados no escalonador.
        * @param 
        * @return ActionResult<List<Processo>>
    
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */

    [HttpGet]
    public ActionResult<List<Processo>> GetAll(int limit = 0)
    {
        try
        {
            List<Processo> list = _escalonamentoService.Get();
            return Ok(list.Take(limit > 0 ? limit : list.Count));
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404);
        }
    }

    /*
        * @description Cria um processo na lista do escalonador informando nome e quantidade de ciclos.
        * @param [FromBody] Processo? pEscalonamento
        * @return ActionResult<bool>
    
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */

    [HttpPost("adicionarProcesso")]
    public ActionResult<bool> CreateEscalonamento([FromBody] Processo? pEscalonamento)
    {

        try
        {
            _escalonamentoService.Create(pEscalonamento);
        }
        catch (InvalidDataException)
        {
            return StatusCode(500, "Objeto inválido.");
        }
        catch (NotFiniteNumberException)
        {
            return StatusCode(500, "Preencha a quantidade de ciclos");
        }
        catch (ArgumentNullException)
        {
            return StatusCode(500, "Seu processo precisa ter nome.");
        }

        return Ok("Processo adicionado com sucesso.");
    }

     /*
        * @description Executa o primeiro processo da lista do escalonador e altera o Status de acordo com a posição na lista. 
        * @param 
        * @return ActionResult<bool>
    
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */

    [HttpPatch("executarProcesso")]
    public ActionResult<List<Processo>> executarProcesso()
    {
        try
        {
            return Ok(_escalonamentoService.Executar());
        }
        catch (NullReferenceException)
        {
            return StatusCode(500, "Lista de execução vazia");
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Ocorreu um erro durante a execução, entre em contato com o administrador.");
        }
    }

        /*
        * @description Atualiza ciclos de um processo de acordo com o ID informado pelo usuário na URL
        * @param int pId
        * @param int pCiclos
        * @return ActionResult<bool>
    
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */

    [HttpPatch("atualizarProcesso/{pId}/{pCiclos}")]
    public ActionResult<bool> updateCiclos(int pId, int pCiclos)
    {
        Processo processo = _escalonamentoService.ProcessoByID(pId);
        if (processo == null || processo.Id != pId)
        {
            return StatusCode(404, "Este processo não existe no sistema.");
        }
        try
        {
            _escalonamentoService.Update(pId, pCiclos);
        }
        catch (NullReferenceException)
        {
            return StatusCode(400, "A lista esta vazia");
        }
        catch (InvalidDataException)
        {
            return StatusCode(500, "Quantidade de ciclos deve ser mair do que a atual.");
        }

        processo.Ciclos = pCiclos;
        return Ok("Numero de ciclos atualizado com sucesso.");
    }

   /*
        * @description Deleta o processo da lista de acordo com o ID informado na URL
        * @param int pId
        * @return ActionResult<bool>
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */


    [HttpDelete("encerrarProcesso/{pId}")]
    public ActionResult<bool> DeleteProcesso(int pId)
    {
        Processo processo = _escalonamentoService.ProcessoByID(pId);

        try
        {
            return Ok(_escalonamentoService.Delete(pId));
        }
        catch (InvalidDataException)
        {
            return StatusCode(500, "Este processo não pode ser apagado enquanto estiver em execução.");
        }
        catch (NullReferenceException)
        {
            return NotFound("Processo não existe.");
        }

    /*
        * @description Vê o estado de um processo de acordo com o ID informado na URL
        * @param pId
        * @return Processo 
    
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */

    }
    [HttpGet("estadoProcesso/{pId}")]
    public ActionResult<Processo> getEstadoProcesso(int pId)
    {
        try
        {
            return Ok(_escalonamentoService.ProcessoByID(pId));
        }
        catch (NullReferenceException)
        {
            return StatusCode(404, "Este processo não existe no sistema.");
        }
    }

    /*
        * @description Consulta o estado do escalonador, como quantidade de processos e se está executando ou não.
        * @param id
        * @return Processo 
    
        * @author Victor, Paulo e Maxuel
        * @date 2022-05-17
    */

    [HttpGet("estadoEscalonador")]
    public ActionResult<string> getEstadoEscalonador()
    {
        try
        {
            return Ok(_escalonamentoService.getEstadoEscalonador());
        }
        catch (System.Exception)
        {
            return StatusCode(500);
        }
    }
}