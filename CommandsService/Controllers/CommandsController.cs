using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform([FromRoute] int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();


            var commandItems = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform([FromRoute] int platformId, [FromRoute] int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();

            var commandItem = _repository.GetCommand(platformId, commandId);

            if (commandItem == null)
                return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }


        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform([FromRoute] int platformId, [FromBody] CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();

            var commandModel = _mapper.Map<Command>(commandCreateDto);

            _repository.CreateCommand(platformId, commandModel);
            _repository.SaveChanges();

            var CommandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = CommandReadDto.Id }, CommandReadDto);
        }
    }
}