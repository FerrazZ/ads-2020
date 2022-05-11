using Microsoft.AspNetCore.Mvc;
using methods.Model;

namespace methods.Controllers;

//sempre ter /api e nome do processo em plural
[ApiController]
[Route("/api/imdbs")]
public class ImdbController : ControllerBase
{

    private static int countId = 1;

    private static List<Imdb> movies = new List<Imdb>();

    [HttpGet]
    public ActionResult<String> Get(){
        return Ok("Olá Mundo");
    }

    [HttpGet("{id}")] // estadoProcesso, com base no id, verifica o estado do processo (ID, número de clicos e status de execução).
    public ActionResult<Imdb> GetImdb(int id){
        Imdb movie = null;  
        try
        {
            movie = getImdbById(id);
        }
        catch (System.Exception)
        {
            
            return Problem("Ocorreu um erro interno");
        }  
        if(movie == null){
            return NotFound("Identificador de IMDB não encontrado!");
        }   
        return Ok(movie);    
    }
//O get padrao deve sempre exibir a lista
    [HttpGet] // estadoEscalonador, mostra os processos que estão na fila e o id que está em execução.
    public ActionResult<List<Imdb>> GetAll([FromQuery] int limit, bool top){
        if(movies == null || !movies.Any()){
            return StatusCode(200, "Não existem filmes cadastrados.");
        }
        List<Imdb> getMovies = movies;
        if(top){
            getMovies.OrderBy(m => m.Nota).ToList();
        }
        if(limit <= 0){
            return Ok(getMovies.Take(limit));
        }
        return Ok(getMovies);
    }

    [HttpPost] // adicionarProcesso, recebe o numero de ciclos, define um id e o estado do processo.
    public ActionResult<bool> CreateImdb ([FromBody] Imdb? imdb){
        if(imdb == null){
            return StatusCode(500, "Objeto inválido.");
        } 
        else if(imdb.Titulo == null || imdb.Titulo == "" || imdb.AnoLancamento < 1800){
            return StatusCode(500, "Campos obrigatórios estão inválidos ou vazios.");
        }      
        imdb.Id = countId++;        
        movies.Add(imdb);
        return Ok(true);
    }    

    [HttpPut("{id}")]
    public ActionResult<bool> UpdateImdb(int id, Imdb imdb){     
        Imdb imdbOld = getImdbById(id);

            if(imdb == null){
                return StatusCode(500, "Objeto inválido.");
            }    
            if(imdbOld == null){
                return StatusCode(404, "Não encontrado."); 
            }            
            if(imdb.Titulo == null || imdb.Titulo == "" || imdb.AnoLancamento == null || imdb.AnoLancamento < 1800){
                return StatusCode(500, "É necessário informar Titulo e Ano de Lancamento válidos.");
            }
            if(imdb.AnoLancamento > 0){                
                imdbOld.AnoLancamento = imdb.AnoLancamento;                
            }
            if(imdb.Titulo != null || imdb.Titulo != ""){
                imdbOld.Titulo = imdb.Titulo;
            }
            return Ok(true);                   
    }

    [HttpPatch("{id}/{nota}")] // atualizarProcesso, com base no id, possibilita alterar o numero de ciclos de um processo.
    public ActionResult<bool> UpdateImdbGrade(int id, double nota){
        Imdb imdbOld = getImdbById(id);
        if(imdbOld == null || imdbOld.Id != id){
            return StatusCode(404, "Filme não encontrado.");
        }
        if(nota < 0 || nota > 10){
            return StatusCode(500, "Nota inválida");
        }
        imdbOld.Notas.Add(nota);    
        return Ok(true);
    }

    [HttpDelete("{id}")] // encerrarProcesso, com base no id, elimina um processo.
    public ActionResult<bool> DeleteImdb(int id){
        Imdb aux = null;
        Imdb imdbOld = getImdbById(id);

        if(imdbOld == null){
            return NotFound(false);
        }
        else{
            aux = imdbOld;                          
        }        
        if(aux != null){
            movies.Remove(aux);
            return Ok(true);
        }        
        return NotFound(false);
    }    

    public Imdb initImdb(){
        Imdb imdb = new Imdb(countId++, "The Batman", 2022);     
        return imdb;
    }

    private Imdb getImdbById(int id){
        foreach(Imdb movie in movies){
            if(movie.Id == id){
                return movie;
            }
        }
        return null;
    }


}