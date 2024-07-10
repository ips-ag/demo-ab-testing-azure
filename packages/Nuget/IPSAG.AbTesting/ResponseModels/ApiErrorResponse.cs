using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IPSAG.AbTesting.ResponseModels;

/// <summary>
/// Api error response type - for 500, 400 and so on
/// </summary>
/// <param name="Message"> Api Error Message </param>
/// <param name="Detail"> Api Error Detail </param>
/// <param name="RequestId"> Api Request Id </param>
[DisplayName("ApiErrorResponse")]
[DataContract(Name = "apiErrorResponse")]
public sealed record ApiErrorResponse([property: Required, DataMember(Name = "message")] string Message,
                                      [property: DataMember(Name = "detail")] string? Detail,
                                      [property: Required, DataMember(Name = "requestId")] string RequestId);
