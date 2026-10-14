using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Data.Data.Entities;
using Usuarios.Services.Contracts;

namespace Usuarios.Services.Business.Mapper
{
    public static class UsuarioMapper
    {
        public static UsuarioEntity ToEntity(this UsuarioDto dto) => new UsuarioEntity
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = dto.Sexo
        };

        public static UsuarioDto ToDto(this UsuarioEntity entity) => new UsuarioDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            FechaNacimiento = entity.FechaNacimiento,
            Sexo = entity.Sexo
        };
    }
}