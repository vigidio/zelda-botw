namespace DataContracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class NewGameStarted
    {
        [DataMember] public Guid UserId { get; set; }
    }
}
