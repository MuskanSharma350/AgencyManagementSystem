using AgencyAppointmentSystem.Business.Interfaces;                                           
   using AgencyAppointmentSystem.Data.Repositories;                                             
                                                                                                
   namespace AgencyAppointmentSystem.Business.Services;                                         
                                                                                                
   public class AgencySettingsService                                                           
       : IAgencySettingsService                                                                 
   {                                                                                            
       private readonly IAgencySettingsRepository _repository;                                  
                                                                                                
       public AgencySettingsService(                                                            
           IAgencySettingsRepository repository)                                                
       {                                                                                        
           _repository = repository;                                                            
       }                                                                                        
                                                                                                
       public async Task<int> GetMaxAppointmentsPerDayAsync()                                   
       {                                                                                        
           return await _repository                                                             
               .GetMaxAppointmentsPerDayAsync();                                                
       }                                                                                        
                                                                                                
       public async Task SetMaxAppointmentsPerDayAsync(                                         
           int maxAppointments)                                                                 
       {                                                                                        
           if (maxAppointments <= 0)                                                            
               throw new ArgumentException(                                                     
                   "Maximum appointments must be greater than zero.");                          
                                                                                                
           await _repository                                                                    
               .SetMaxAppointmentsPerDayAsync(                                                  
                   maxAppointments);                                                            
       }                                                                                        
   }      