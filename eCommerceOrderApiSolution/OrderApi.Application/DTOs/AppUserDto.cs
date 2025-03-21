﻿using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record AppUserDto
    (
        int Id,
        [Required] string Name,
         [Required] string TelephoneNumber,
          [Required] string Address,
           [Required,EmailAddress] string Email,
            [Required] string PassWord,
             [Required] string Role
    );
    
}
