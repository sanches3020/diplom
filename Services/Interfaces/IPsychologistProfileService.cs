<<<<<<< HEAD
﻿using Sofia.Web.ViewModels.Psychologist;
using Sofia.Web.ViewModels.Psychologists;
=======
﻿using Sofia.Web.ViewModels.PsychologistArea;
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab

namespace Sofia.Web.Services.Interfaces;

public interface IPsychologistProfileService
{
    Task<PsychologistProfileViewModel?> GetProfileAsync(int psychologistId);
}
