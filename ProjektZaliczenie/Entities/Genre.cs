﻿using System;
using System.Collections.Generic;

namespace ProjektZaliczenie.Entities;

public partial class Genre
{
    public int Id { get; set; }

    public string? GenreName { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
