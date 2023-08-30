using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        public static List <Character> characters = new List <Character> {
            new Character(),
            new Character {Id=1, Name = "Sam"}
        };

        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>> ();
            var character = _mapper.Map<Character>(newCharacter);

           // character.Id =characters.Max(c=>c.Id)+1;

            _context.Characters.Add(character);
            serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>> ();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = characters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c=>c.Id ==id);
           serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
           return serviceResponse;
        }

       public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto> ();
            try{
                 
                var character = characters.FirstOrDefault(c=>c.Id == updatedCharacter.Id);
                
                if (character is null)
                    throw new Exception($"The character with the id {updatedCharacter.Id} has not been found.");


                _mapper.Map<Character>(updatedCharacter);
                //OR
                //_mapper.Map(updatedCharacter, character);
                //
                
                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defence = updatedCharacter.Defence;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                
            }
            catch (Exception ex){
                serviceResponse.Success=false;
                serviceResponse.Message = ex.Message;

            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>> ();
            try{
                 
                var character = characters.First(c=>c.Id == id);
                if (character is null)
                    throw new Exception($"The character with the id {id} has not been found.");

                characters.Remove(character);
                serviceResponse.Data = characters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList(); 
                
            }
            catch (Exception ex){
                serviceResponse.Success=false;
                serviceResponse.Message = ex.Message;

            }
            return serviceResponse;
        }
    }

   
}