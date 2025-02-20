﻿using System;
using System.Collections.Generic;

namespace ProjektZaliczenie.Entities;

public partial class Game
{
    public int Id { get; set; }

    public int? GenreId { get; set; }

    public string? GameName { get; set; }

    public virtual ICollection<GamePublisher> GamePublishers { get; set; } = new List<GamePublisher>();

    public virtual Genre? Genre { get; set; }
}
