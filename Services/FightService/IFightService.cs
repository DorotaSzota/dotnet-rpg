using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultsDto>> WeaponAttack(WeaponAttackDto request);

        Task<ServiceResponse<AttackResultsDto>> SkillAttack (SkillAttackDto request); 
    }
}