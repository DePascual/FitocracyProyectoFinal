using FitocracyProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitocracyProyectoFinal.ViewModels
{
    public class WorkoutTracksTablaDatosVM
    {
        public Workouts workoutVM { get; set; }
        public Tracks[] tracksVM { get; set; }
    }
}