using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
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

            _context.Characters.Add(character); // we don't make this async because tis is not a query
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>> ();
            var dbCharacters = await _context.Characters.ToListAsync(); //we make this async because this is a query
            serviceResponse.Data = dbCharacters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList();
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
                 
                var character = await _context.Characters.FirstOrDefaultAsync(c=>c.Id == updatedCharacter.Id);
                
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

                await _context.SaveChangesAsync(); //this is sufficient to change a character's properties
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
                 
                var character = await _context.Characters.FirstOrDefaultAsync(c=>c.Id == id);
                if (character is null)
                    throw new Exception($"The character with the id {id} has not been found.");

                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToListAsync(); 
                
            }
            catch (Exception ex){
                serviceResponse.Success=false;
                serviceResponse.Message = ex.Message;

            }
            return serviceResponse;
        }
    }

   
}