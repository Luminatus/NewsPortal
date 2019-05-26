using System;
using System.Collections.Generic;
using System.Linq;
using NewsPortal.WebService.Controllers;
using NewsPortal.WebService.Models;
using NewsPortal.WebService;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Web;
using System.Net.Http;

namespace NewsPortal.WebService.Test
{
    public class NewsPortalTest : IDisposable
    {
        private readonly NewsContext _context;
        private readonly Editor _user;
        private readonly List<ArticleDTO> _articleDTOs;
        private readonly ArticleListDTO _articleListDTO;
        private readonly List<ImageDTO> _imageDTOs;
        private readonly FakeUserManager _userManager;
        private readonly SignInManager<Editor> _signInManager;
        private readonly string testImg = "/9j/4AAQSkZJRgABAQEAYABgAAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCABpAMgDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9zfE3iPVLfxRcW1rcRw28MMTAfZ97FmL5yc+wqp/wkeuf8/kf/gIP8a5z43Wluur67ePa2881vpsDKSYkkGGk4DyRuF6nk5Az0HUeHa1dz6IQ15cXEPyyxu32+2whwp5I0oIrBdxGV5wMN2P0GDwMatNOyPXw+FjOCZ7hrv7QVn4Z1m407UPFOm2d/aoJJbeS2xIqlQ24L1YbSCSMgA84qFP2ktLkmWNfGOitM23bEIh5j7hkALncSRg4AzgivF59TeFphJrF9arJMUm8y7t4vs4jBZmTdpG0KACD0yApLZLGusv/AIb+JtRnvprWC+jtb6bzbdTqdtb/AGdGkLArG9gZFAUhdkjkgdVVsgdby+hH4kvvS/Q3+qUl8S/I9X8N/Ey58ZaRHqGka5p+qWMpIS4tYUljYg4IDBscHg+lYmqfE74jWeqXEdv4at72zjfbDOmowRyTjcRuMbKAo24b7xPOMcc+fv8ACvxXqFwsskN1DsZZfL/tbT2VmHGwkaZkqQckE4+XpzXZfDj4dHSvDirrNjDcX7uxY3Dw3bKo4Ub0ghTp6Rg88ljzWUsJRjraL8t/yaIdClHWyf8AXk0epfDHXNQ8R+C7W71aOKHUHeZJ4ozuWJkldNuf4sbcbhgEgkAZwJvHGr3ek2Vr9jeOOS4uPKLNHv2rsduBkc/KB9CaqfCS0isPBKRQwxwRreXmEjQKq/6VMeAOKg+L9tFeaPp0cyQyRtfLlZQCp/dyddwI/T8uo8Plj9Y5baX/AFPM5U63L0v+pR/4STXP+fyP/wABB/jUd14t1iytZJptQjjihQyOxtBhVAyT17AV4f8AFrUbfR/F89qvl6fbxRCQiGe2txIp+X5U+xXEm4HPO5ckjgYwOftL6abRBcfbGEe42rONRtvLEqqDhmGll0H7xG3E5YlBuYE59+OWxcVKy1PUjg4tXt+B7M37T2ixpEzeNtBVZvuEqozyQc88AEYJOADx1IFSD9pXSTEsn/CaaD5bNs8zanlhuuC2cDt1PcV5N4Yu7n4hobG1up5NUtla7jW1voFNwC0aujn+zo1WKPD7XZdxLLnO9RXTeFfCfivw1LJLdaLZ680kSoqXms24W3xz8vl2UYZj0ZmDEkZXYCVLlgaEdHHX1X+RUsLSW6/I9Zs/F+rahZw3FvqME1vcIssUiWwZZEYZVgc8ggg5rnfEXxJ+IOmatcR6f4ft9WsYUDJOL6G3lnOASqxsMAgkgbnAO3qMiufltdeFwMeAfCckRjyx/tVQ4k3HjH2XGCuDnPU9OOemsPCthJZQtc6RpEdwyKZUit0dFfHzBWKgkA5wSBn0FY/VaUdXFP8AryZl7GnHVpf16M7H4TeJNY8S6DdSa5bw2d9Dc+UYI3EnlL5cbgFhwx+cnIAHOOcZPS3krQWkrrjciFhn2Fef/DrxLpfhS/1PTFt5oWmvRKqWtk7Kc28PQIpz91skAhcDcVLIG7iS+i1PQGuIW8yG4t/MRsFdylcg4PI4NeHio8tR2VkeZWjabstDitN8V67d6dbzNeRbpYldsWgxkgH1qf8A4STXP+fyP/wEH+NeF+N9RXSPCuixxvp9iLhCReW0Gk22WikEbRtJdXBQ567UjLDGGKHK1zdj4gj0y5uJJL69vLVk/ei4vPDjfZTuJeQrHc71VeOVZgBuGwBVB+hjl1OSurHsLBweuh9Ga/8AES+8LaTNf6lq1vZ2NvgyzyWn7uIE43Mc/KvPLHgdSQKw9N/aQ0vV7i1ht/GWhyS3zrHbpsVTOzMFVUyRuJYhQByWOOvFeF2mpyT3lu8GqapqEkRQeVHfeGV+3bcExqEnU/dZi7jynCjKsxCEe26VoN1qGnrNdafolhNIWD28mlq7R4YgZKzFTkYPBI56nrSqYGjT+JJ+n/DEywtKK1Ov/wCEj1z/AJ/I/wDwEH+NY/i7x1410mGA6NY2utSSMRKskyWYiGV5yQ27gscAdVA4zkZv/CGL/wA+/hz/AME3/wBto/4Qxf8An38Of+Cb/wC21isPRTvyr7jNUaafT7je+FHj3xZ4i8Wy2niDSI9Jtvs0skA+0RTvOyvEN2Y+FUB+h5Oe2OfRq8r8D21p4Q8e291dyaHYwtp1ynmx2i2fJltsAsXIOcdK9UByK8nHRjGp7qsvL/gnBiopT91aBRRRXGcx4/8AG3Q7XUPE+r3FzA2yLSo1M8W8TRkyErtKxuR0b5hkqCflIZiPDrSC8imtoriRluJN6yMt3qYBTC7cZ0tVVyxI6M2FyARu2eqftR6Xbz+JLy7uLq0tY7Owgfc129nI5M/lhTKpwEzKMZ/iwOd5K8EnxkWxtWjj0rwRcLbgQq7XRkZ32ggOQpIOMsWbgKN2SCK+uy+LdBWV/wAD6HCfwlYzEmuJQ21n86VjF5Zvb+MSD5iWB/sglCMbQoUE4HzMSCd/TNG8M28ekPfat4uaSZBcNBFZySxzKjMNshW2EiBth3fdYkkBuDg0r4uNrENxJDo/gNo7WNJZmW4VljVsKCTjH+t3IB1ICnGW2De8L6lrfiuyN5beFvCcmnzB2tbiJkkW5C7x0yMZZQM8j5s5wMnoqXW6t80bS08vmdZ4Y0XQdb0C1uLOzs7y2K+Ws0tkFkkKEoxYMgIbcpzkDnNXv+EQ0v8A6Bem/wDgIn+FY+h+GrzUbHzNQ0nw3p9xvYeStgLj5R0bcJAOfTtVw+DAP+Xfw7/4J/8A7bXC9zle+50vwrttX0bw5cR2VnpUli2oXbW6NcG28hfPcMu1YWHLh2zkn5/YCqvxgk8QTadpchS0sYYLxpJPstzJcSTD7PNtUD7OSPm2klQTgEYOeN74N2X9n+BI4ttuu28veIIfJjH+lzHhcnHX161T+O15FYeE7aW4Fu0K3YEgm37GUxSAj5FZsnPGATnFeJH/AHv/ALe/U8yP+8fP9Twnx7cahoniPU4XaeYRTvcQyy6hqafLLGYnYxw2EiFQgyFRvLyTtKsrNXOInnQqsjalbyW6tJFjxLr8hicEhsAWC8YZgNp4XgfLwNiyim8e+L7jXNH1TwtprQ3yXajU7DVI5TleCPOmiQt94ErHgDAZAduNnwvoniLwVBIun+KPAOZsG4aRLlpJ8MSPma4YIQpYAhSAW3FWxtr6hOEY26/NHvaJW6/cRaR8NfExezvreGzmjUJdQLeeML66UOdpV/3tmw4HO0x8HB3ZyB1EUPjyXRjMw0eHUPPwtq2qBofIAPzNMLLIkJ2/KI2VQD87ZGHWHiLXE1eOS71zwTPYiPDW8MjxTO+0DPmlmGN244EY4IGeCW1v+EzX/n48O/8Ag4/+1VhKbe6T+/8AU55Sk3sjEeP4hLO+2Hw+0XITOuMrDGME/wDEtIOeeBjbjq2chGg+IhuZAv8AwjqwtIqxMdXZmRMfMzD+zwGYHkKCoOMbhncNz/hMx/z8eHf/AAcf/aqP+EyX/n48O/8Ag4/+1VPN/dX9fMnX+Vf18zsfgzFcQ6TqgupWluDeoZCdvyt9lt8gbVUEA5wcZrX8MDPw808H/oHR5/79CvPvDEtx4huLw2djoerXXnXHkxzXfmWYYRWHzM4jJBALfdQnJxwCSOj0QeOLeeG2u9H8H2eirAY2jsdQuHmtwEAVY1aBUYDB/uYGB2yfn8TBucnp96PKrx95s8R8Taze6v4J0WOSLyHaOaVbk3moW7RkTSxqF/0GXIKhSC4XjDR7lVXrF0K/hcR3GuX2tW9rEhbzrf7dO126dQkcumxhU3qRuD5OwZ466mofCDXlubWaDStUmimjjWeKFry1jt8Fs7Y9n90IcBxy5XjbvbcjbxiSv/Fu9IX5Mk/ZZThsfd/1ecZ4z9CQO303tKSVov8AFK33nuc8LaP8UdZ4e1DQfEmrXzWNjH9rt1WK4lfSpLcsvzAKHkjXzFHzfdLABvRhnoMf735VxuiaNqV+Z/7Q8M2OnhGAh26BPceavPJxjaenHPXqcVe/4RY/9Auz/wDCVuP/AIquSVSF9/1OWXJfc6TH1/KjH1/Kub/4RY/9Auz/APCVuP8A4qj/AIRY/wDQLs//AAlbj/4ql7SHcXudzptB0q31fxzZx3VvHPGtjcuokXcAd9uM/kSPcEjoTXVeB7eOy0FoYY44Yo7y7VERQqqBcSYAA4FcX8MfD8lh8QYrj7BDbxrp1xG0keiy2XLSW5ALuSDnaxx7Z7VrL4Bm8WWssv8AwkGv6UguruLytPnSFWH2mQkncjHceRuBBAxjBG6vJxjUqm+mn6nn4izna522aKxfB3g9/CEVwjaxrWridlYHUZllaLAxhSqrwepznmiuGVk9Dme+h5r8fYLifWNW+zrfM/8AZ9uEFvDeyYk+0oVOLWRX4xn5RuChjuCBw3ks+na9Lp2yGLxZBKse9luY9feONsgKyv8AaQNwRWwcjYdrZUj5vqzU/B2ka1efaLzS9Nu7gqE82a2SR9ozgZIzgZPHuag/4V14f/6AOjf+AUf/AMTXrYfM1SpqFmejRxyhBRsfOuneDdc1y8ezZ9ci1KOFZBdzX2uw2u0KoeMSPPkktsOzJDBXJ9D2A+DGn3Nki3F54k8whS6w+IL+OFXBDExp52FwwypA3D1zXonjz4d6IngvVWg0fSYZo7WSRWS1RDlVJxuUBlzjGVIYA5UqcEaNj8NdBt7OKOXR9IuJI0CvK1hEpkIGCxAXAz14rSWbaX1Kljr6q55fL8G9LnEfmXniqQwyiZC3iPUMhxjB/wBd7Dg8deOWzNZ/CvT7HTZLWO58Q7Jtu+RtZvDO23djMvmb8YYjG7GABjgV6d/wrvw9n/kA6N/4BR//ABNL/wAK68P/APQB0b/wCj/+JrP+1L6a/eR9e9Sh8G9PTSPh9a2kbTNHaz3UKNNK0sjBbiVQWdiWZuOWYkk5JJNQ/GZHk8OWyx+Z5huTt8sSFs+TL0EbK/8A3ywPvXTaXpNrodkttZWtvZ26FmWKCMRopZizEAccsST6kk0apo9nrlt5N7a295DuDeXPEJFyOhwQRmvP9t++9r53OT2n7zn87ny7rmi65deIte222tMsN1czR+a+tPGY5JmQCBUmClsbWzEoVFY7cKSateHvhLrurW1teNdXVvDhzHHNq+uW1xFyRja1zlN2B1GQDnBJwPof/hXXh/8A6AOjf+AUf/xNH/CuvD//AEAdG/8AAKP/AOJr1P7Y0skzv/tFWskzxGT4Y+Jn0q3ibU7d7iOd3d/7S1NI5IyI9oIWcFjuQ53k5BOCNzA+iNuZmOH610mo/DLQ7vT54odI0i2mkjZEmWwiYxMRgMAVwcHnB9KoeB/h7ocng3S5JtF0mSSS1jkYvapIRuUHG5gWbGcZYljjJJOSYlmSau0RLGKS1MnDf7X60Yb/AGv1rR8b/D/R4LC3e38O2MrLKxK29grM37qTA+Ur1baMk4BIzj7ypLp/h6MMf+ELZgu48aRH8wG88DGeQgwOv7xBwcgR/aMexH1qPmReB9Jurm/1C6tbiG3mt7t4SJoDKrK8FqegdSCNg7nqa37+DXIrGZhqGlhljYg/2e/HH/XaquhaxpujalDptnosul/bZXbC28dvG5UMC+Mgt/qwMqCcNGThSDWt4i1W10nSpGurm3tVkBjRpZBGGYg4UE9zg8e1edWqc8nI5KlTmlzHlt18QofD2lae99b+JN11FuBil1S4J25DEiOJuMgnOOQQwyCCXWHxLtNRiZo7XxGrRtGkiNLqYaMyByuf3eCMIckEhcrkjcueS+J/hn7RoegyXGlW6wfZ7iN5DYwbVLSFlGWv4hlgS+NshY/MQhBSua0TxRfaNe6dNazrDNp8piXytPsJTAJJAXzu1WRk3LKrEsThdzDaMAeB7SaZ866009T1rxZoPjK7uVbQ72xsItm10vbq8uiW5ywZJ48fwgAgjqeeBU3hXRvFdoLga1Na32SPs5s9QvLdkGWz5m6RwxI2cqFGQ3GCAI/h38VrTXrvUYbzWNPdmvdtmGmtYyFcDFuBHPIXkjb5WLBCSwAXAydw/E3w2sXmHxFoKx5A3HUIduTgDnd3yMfUVfNLe5XO97ifYb7/AJ97z/weXFVtUi1K0it3hS5gkN5aoHfV55lUNPGpyhIDDBOVyMjIyKuXHxH8O2nl+dr+hw+dIIo99/Eu9zwFGW5Y5HA55ouPEOn+ItNhk0++stQjj1CzVntp1mVT9oiOCVJ5xzVRnK+5cakr7nQiz1of8v8Apf8A4L3/APj1WdD0xtI07yWkE0jSSSu4XaCzuznAycDLHAyeO5q5RXrXPZuFFFFIAorznx1cWZ8e3iXVvdXDLa25TZZzTqoJl6FFIB9uvT2rFOu+HVKAowMjbUBsZvnbBbA+Tk4BOB2BNd9PAucVK7+4nmPTfGpx4N1bH/PlN2/2GrAf4EaDPcCWSbxNJIrBlMnibUn2EAKCuZ/l4APGOcnqSa8l8QfE1LPVLqxi8D3GoQsGWCVp4oo7xOjfI43gAEkgqRgH2yyb4sSieOOP4e6pI0jrnNzbjahO3djdkndkADrtbnICt3U8prpJxla/nFf+3Aq1tj1eT9nrw5NLG7zeKnaFGjjLeKtUOxWUK2P9I6kDk9e/U11mg6HB4b0e3sbVrloLVNiNcXMlzK3u0kjM7sepZmJPc14v4P8AGum+I9KkuL7QbzRZFneJIZYftLSouAJN0IdRk5G0nIKnNY9/b/2vra3Gn+NJLazuZv3NonhtLgFTjCK5Tczbo5BnngsMZXcIeW1ZNwqT27ptfhcJVm1rqfRlFcn8Edy/Dm1SSae5kiuLqJppoDA8pW5lUsYyBszjO0AADAHGKk+K0yRaXp4mWaSGS9CyJHG8hceVKQCqAkgMAenGAe1eW6P710r9bDvpc6iivI5tS0K3bbJbTxt6Np1wpP5pXP8AiX4jaTpMdrNp2iSa1ZzAm4uYsQR2QOzY7mQLlGDk5TJwBgHcK7oZXObsn+FvxbsLmPfK8/0D4NaX4g0KzvL6+8TzSXltbu8SeIL63t0CxRAIsMUqRhDsGRt+bc+7O5s+W/8AC4mhiaSb4d61AsaiQs1zaYRTzlj5nHcdxkcEjmp9B+JkVxqEVtN4HvNO0/eUS7WWGSFIF4Eu1Pm24HQKSNy/7W3oWUV4JuMvucf0l+ARrW2PVX/Z98OyWklu8/iuSGRVVkk8Vao4wv3etxxjPb29BU03w80/wJ4R8RSWEmsSPe2L+ab7V7u/xsjcLt8+R9nU524zxnOBjzrxPbW/im0jXQ9WvNFkj3iSW30FrwvkYHEkRClW56EHkEekvgnTrzSZ9ThvNWu9Vlm0C+JL6MdNjBVouQNoDE78AEkgLx1YnKtg6ipOUqnyal/lb8RyrSas7noeueD9Ij8XaGo0vTVVzOCPs6AN8gIHUA9M4wx4zgYLLo6r4F0V9OuP+JTpq/uXUlbZFypHKnA5UjgjoRkHIOK5Hxp8bNDs/FcP2e+mmfRGkjvWgsJrpYmbb+7OwDLFQ7cMQNgyjEpirc/tG6YdJkmmnvTDsUSBNAuWcbgg+4rlzyzdAfuEdSm/xeaHdEc1Pujzr4nvBBo3htoZYbqZfN+zC/1GwkxcLI0cYTz7lCpByN0e5hgAqGL45QajbvdRx/bNNushnBeXRN1yq8ny8X/7wAHcSfLAEud2QUr27wyi+OFabTptRupdKU2gluNHgR4cr9wNIV3DOzOwkHy25+ZSL2leHIbnxRc6Kl7HZ6hbW8d8YH0603eUXmjjfCE9NoGSAD8+OpCed9Tk9bnl/UZS1TPGdPSa6v2uob6zt7tZIoV8+TTG23DGTZ+9hu2YSNtIwcE4GxeHFbmgfB7Xdc1WSC8sdU0ezuZC5kutP0ySOIGPDcRXUjbnIUZIf7iZ4BB9kj+GE0LDZqcSqoC4XToB8oJwOnYFsehYn1BsJ4K1TK7tfkbpuxZRDP3c4/J/XG5eu07q+pS6lf2fL+mc9YfBvw/a2EMUun288karvkyyeY4GC+0HAJ56dM1PceC9O8P2FnHYxzWsMeoWjrFFdSrHk3URJ2btpyeTkc962l8FaptGfED5wM4souTgZ/Xd+Y64JLZfAeoThPM1xpBHIkwVrNNpZG3rnaQcbghIBGQCMjORccLJPoaRwc0+h1FFVNIup7hbhbjy98M7IGRGUMvVeG74IBIJBIzxnAt13npBRRRQB498aLL7X4y1hY76TTpm0ZCs6zmMIVkwMj7TDkfOR2U7hmRCFV/nTSPC2l3Gl26Lofh8RwiSBkfQdPMduFIYs2dYJLFmwTuwWDFgDlq+gvj14r1Dwz4yvPsWh6pryz2lvG0NppCXoiy8nLF7iEHJwNoJxgE4GCPL/Cvwoj8QaZdXlzqniTR45HELw3vhjTJjKqttDFoIpV5dWABfIYMQFYmvuMpxEqWGTcklp5v7krnPUjdnJyeE7fy5JE8P6G1q0ge2K+FbMojgENHGP7YBMjAJy393oAVA9i8G/BTwv4j8Lae2reDba2uLGN4I4bu28trdHDFlRQ7KqkSurBThiX7MaxZPgrIVs0bxh4i8yzlNzbsfCdtuRioUkZtecgLknOSqknKqRrDwhrwC7viF4wk2tuy/h1Ruxg4OyBflODnGDzwVrbFYx1YpQqW9FJeXRdtxRika0X7OPgOC4aaPwnoscrEszrDhmJGCSc5yRxn0q3oPwN8I+FdQS70rQbHS7uNHjSez328qK5BZQyMCAxAJAOCQM1T0Ky1bRtWuLqXxRrmpRTl9lrd+HpPJttzBgE8tEc7cYG9m4Jzk4I1/7cvf+fr/AMt29/8Ai68udat8LqNr1l+pdonbfCS2W08FiNTIyrfX2DJI0jf8fc3VmJJ/E1H8Vtp0/S9+3b/aC/e24z5UmPvcdfx9OcUfBqd7nwKryNvdr6+yfs72+f8ATJv4HJZfxPPWoPjXPNa+HbOS3ZVmju96l9+04hlODsBbB9gfoa8PlbxbX95/madD5v8AjxJp9r8X7hb2LQbi5migMVvdTabHdTA7ljCtNFNcAF8HEe3/AFYwp3Fag8HW+h3vnSzXmgK99fot/eQpozSNE26QIfs9sBKJHjlJjfJU2rNwxVk0Pin4h1LSPGerQ3Gr3Nm1u/mIINevdLjMTKR5gihj2tjbvZg4OSVL45bBi8fXUMbS2upSwXFvD+6tX8Y6rsXaw3rs8ouOexXkrg8HFfeUYzlQglHolfTy7v8ArrtY5noz06x/ZisbCBlbULdpdy/OnhvSYOP+WgPl2yn5z8xwQN3UFSULrD9mrT7K++0/2hbtMoAEg8OaOkjcKh3OLTJzEoj4x8vBzgV5xpHiOfxb4p+z2d9cQzXh3x29v4r1OLaZXUPhNiqjFnYgZHOCCnFfQTWepFj/AMTG26/8+X/2deXiqmIou05avul5drlxSZU8GeBbTwTYLDblJWVQvmfY7a3PRQ2BBFGvzFFJ46gdgANBdLm1rxPFaQzR2/2nS72KR3iMnys9sDgBlweepz9Kh+xal/0Ebb/wC/8As6isPBsfivxpZ2utNDqFqtjdSLGsJh2sJLYZyHPYkfjXkV2pRk5vo/66Gh2OsfFnwt4duZoL/wASaFZ3Fs5jlimv4kkjYKGKlS2d21lOMZww9RUC/FjwvroazsfEmhXV5cRfuYYL+J5ZNykrtUNk5AyMVLB8I/C9tOsseg6XHMpYiRYFDgsQWOevJVSfUqD2FLcfC7QQk0kWkWKXDq2JBHtbcRycjnn1ryf9n5be9f5f1+JWp5J4kvoLXwXCslv4digayZ2uYxbWboYriKAqJGuMLkv8xByv3Th2UVt+FPgdLpN1FfPrX9peYgYwXunQSRNleM+XtzjJxg4ySeSc1LqPwm1K/sLNIZfEWmy28XluYl06VWBJPR8gFflVWAyFTnJJJbD8Jtajky2peJGXYEC/Z9MABGfmHv09vUEYA+f9jO/wnj+xqXvys6TQtNi8JeKlurhdJt4I9NupJHtbH7PtVXgJLHe2QBniuL8XeJzq8niu8t/D80k0dykMSX2kGKaUmOKM7JnYKuBh1LDByAOcgdZpPhW60LTJpZYbuOS00e5glu53jWS4dvLIbEUny/cJ4K4457i14t0Dw9q2q6pDfSXExvvLS+gGnJdxSbVUorboXyANrbScAnOBmur2TdLl21O2OHqTpckVqeEy6NqUkEMckfi63mwxQNDpySZHVnAJBC5K8b1O3JB4au+8JQotpb28lrAZNO1m0hjuJ9HMlxcx/aoikxkjO2POcDeAwCgsK1m+Dnw9YYOj2p5zz4Wtvb/p09h+QrU8PeEvC3hsxrpzXenwzXduXSDSUtYp5VlXyw7Lbr/HgZJB5xmsKeGkne6+8yhlteLu1+f+R22hbfMv9u3/AI+mzjHXavp/XmtCqGhvvkvvm3YumH3s4+Vf9o4+nH0He/XqHpBRRRQB5t8Xfg7f/ELU757f+yfLurOK3VruKObyyk6TZKSQSBhmNMDcBnkruRGHnA/Yjvk1D7UknhWOVQAu3RNPTbgKAcrZB8jy48fNwEA54I+kKK9PD5viaEOSm1b0T/Ml009zwfQP2Y/FXhrWrzUrPWtCs9S1FX+1XVrplnDNcsxB3SMLTLHcNxz95ufatKL4N/EZLXy28bRs21VMv2e1EmQuCR/omMk/NyDzkdK9mopyzjES1kov/t2P+QezR5HpXwl8f2VxvufFVvfKEVVjeOCNchsljttQSSvHBAGMgVqeGPAHi7R7WZNRvtP1qSSTckskwtzEuANmI7cA8gnJ559q9IorOWZVZbpfKKX5BypGP4E0a60Dw8Le8EIuDcXEzCFy6ASTySAAlVJwGGeBzn61j/GmOSXw5arEJDI1ywXZu3f6ibpteM/k6/Wuwri/joI28J2/mtCsf2htxlMaoB5E3UyK6f8AfSsPascLJyxEZPdsJbHgvjCbV4PiTrzWMGuMzO6r5UOoOkiElMKY7xdgwX2mBUxINzbflUTeBviL4o8PwrZ28M09g16ztPNpV+++P5N+15rqRwQBIdrbiADgKoBrj/itdaEfjDrS3v8AwiSyQz3LuL19FEiu6LEZGE9q8hJjLJ+9dEwMFpX+Rs3T/Eej2GoqtrceEory4nEcMcU/h+O4lJyNsgjtwrlg0iBEeRsjB8tsFv0CODlOhG9ndJ6/1+PY5eazPoXwR471LWmW31LT9TW4mmlKv/Z7W0VtGo+QS7pXO888jhsjheRXU184Wnje1+Hr3ltoevaN4Ztlk86TTLe80S2YNOgePYv2YCViW2puIGEwDIQQfX/hB8YrP4v2F5Na/wBlo1p5ReKz1aLUNokDFSxj5UHawG4Ako3AKmvFxeAqU17VL3fL8NP8rm0ZJ6HYVJ4Z/wCSg2f/AGD7r/0ba1HTdH1CPTvHdlJIszL/AGfef6qF5Tw9u3RQT0B+pwByQD5db4H6Mo76iqLeIrdWK+XfcEjiymPQkf3f9k/oehGRfEVuzBfLvuSBzZTDqQP7v+0P1PQHHhmheorPTxJbybcR3/zYxmxmHXZ1+Tj7469MNn7rYB4ktyufL1Dpn/jxn9Af7nv/AE6g0AO8UKX8M6iqhixtZQAAST8h6YBP5An2NcD4s8R3mga74nuI9NN5BYulwQkriSb/AEeL5EURkM/y9M5+Yetdh4o1qG68N6jCsN9I0ltKgX7BM247H4x5bZztPG1s5UYO5QeZ8XeAbnVtc15m037ZDq4hj+aC2mieNEX5XWRldgW3AqTtAOV2sSayrXa0O7AyhGo3Pa36o810nW/iBqUPyX90TavEJHk8OtEJg+zAAYc4BJcrxh1xgqQ2v/wgdv4Wt7PWLrRNHvNSs72C6nvrNpWmnY3aySPHCsZ3Oxd8KDkk4HYDL+GfwGhisp5LXSxusbw20MkfhvSbPyjCxjl2jkMsihk3rt+QgdQa9Mi8MXcFpY2dnoclja211bMqKbeOGCOOdHOFVzgBVPAFc0act5I9mvi6W1OSt5af5f18jqfCV82q6a94Ybm3W6kMqR3ClZEBAGCCTjBBHHB6jIIJ1KB0oruPmAooooAKKKKACiiigAooooAK5r4oaVcatpFmsENzN5d1ukFu7JIqGORSQVZWBywGVORnPTNdLRV06jhJSXQD5t8b/BXxfr+r6rcac17axX00kkUUmq6xFEu4A5aKC8RB86ocRhQApwV3NTofhD44eZmuJr6ZLiHyrlTrGsMrcEHYrXRCBs477RyvIFfSFFev/buI5VDSy9f8zP2aPna1+GPjrT102SF7hrjTsqFfUNU+zSBHDw74/tP70gvMH8wtvAiBOFxXpcsl9I3OmaswzxujBx/49XfUVz1M0qVNZJf8ONQSPP8AF5/0CdU/79D/AOKqTQ9CuNV8X20lxY6pa28NlcxtJ5jQfM7QgDKOCflDkdgVB4IBrvKKxnjJSi42Q+Uz28N27sT5mockni+nHUk/3/8AaP6DoBgXw3bowPmahwQeb6c9CD/f/wBkfqOhOdCiuMoz08MW0e3EmofLjGb+c9NnX5+fuL165bP3myDwxbKu3zNQ6Y/4/wCf0A/v+38z1JrQooAz38MW0gYGTUPmBBxfzjqGH9/j75+mF/urgbw1buW/eah8xJOL6cddx4+fj756dML/AHVxoUUAQadp0elWvkwmYpveT97K8rZZix+ZiTjJOBnAGAMAAVPRRQAUUUUAFFFFAH//2Q==";

        public NewsPortalTest()
        {
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase("NewsPortalTest")
                .Options;

            _context = new NewsContext(options);
            _context.Database.EnsureCreated();
            

            _user = new Editor
            {
                UserName = "admin",
                Name = "Adminisztrátor",
                Email = "admin@example.com",
                PhoneNumber = "+36123456789",
                Address = "Nevesincs utca 1."
            };

            var userList = new List<Editor>();
            userList.Add(_user);

            _userManager = new FakeUserManager(_context);
            _signInManager = new FakeSignInManager(_userManager);


            var adminPassword = "qwertz";
            var adminRole = new IdentityRole<int>("administrator");

            var result1 = _userManager.CreateAsync(_user, adminPassword).Result;

            // adatok inicializációja
            var articleData = new List<Article>
            {
                new Article
                {
                    Id = 1,
                    Name = "TestArticle",
                    Lead = "This is a test article",
                    Content = "This is REALLY just a test article",
                    CreatedAt = DateTime.Now,
                    HighlightedAt = DateTime.Now,
                    PublishedAt = DateTime.Now,
                    Author = _user,
                    Images = new List<ArticleImage>()
                    {
                        new ArticleImage()
                        {
                            Name = "TEst image",
                            Image =  Convert.FromBase64String(testImg)
                        }
                    }

                },
                new Article
                {
                    Id = 2,
                    Name = "TestArticle 2",
                    Lead = "This is a test article too",
                    Content = "This is REALLY just a test article too",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    HighlightedAt = null,
                    PublishedAt = null,
                    Author = _user,
                    Images = new List<ArticleImage>()
                    {
                        new ArticleImage()
                        {
                            Name = "TEst image 2",
                            Image =  Convert.FromBase64String(testImg)
                        }
                    }

                },
                new Article
                {
                    Id = 3,
                    Name = "TestArticle 2",
                    Lead = "This is a test article too",
                    Content = "This is REALLY just a test article too",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    HighlightedAt = null,
                    PublishedAt = DateTime.Now.AddDays(-2),
                    Author = _user,
                    Images = new List<ArticleImage>()
                    {
                        new ArticleImage()
                        {
                            Name = "TEst image 2",
                            Image =  Convert.FromBase64String(testImg)
                        }
                    }

                }
            };

            _context.Articles.AddRange(articleData);

            _context.SaveChanges();

            _articleDTOs = articleData.Select(a => new ArticleDTO()
            {
                Id = a.Id,
                Name = a.Name,
                Lead = a.Lead,
                Content = a.Content,
                PublishedAt = a.PublishedAt,
                HighlightedAt = a.HighlightedAt,
                CreatedAt = a.CreatedAt,
                Images = a.Images.Select(i => new ImageDTO()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Base64 = Convert.ToBase64String(i.Image)
                }).ToList(),
                Author = a.Author.Name,
                IsHighlighted = a.IsHighlighted,
                IsPublished = a.IsPublished
            }).ToList();

            _articleListDTO = new ArticleListDTO()
            {
                Page = 1,
                PageCount = 1,
                Count = _articleDTOs.Count,
                Limit = 20,
                Articles =  _articleDTOs.Select(a => new ArticleListElemDTO()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Lead = a.Lead,
                    Author = a.Author,
                    CreatedAt = a.CreatedAt,
                    HighlightedAt = a.HighlightedAt,
                    ImageCount = a.Images.Count,
                    IsHighlighted = a.IsHighlighted,
                    IsPublished = a.IsPublished,
                    PublishedAt = a.PublishedAt
                }).ToList()
            };

        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();            
            _context.Dispose();
        }

        [Fact]
        public async void GetArticleTest()
        {
            var controller = new ArticlesController(_context, _userManager);
            var result = await controller.GetArticle(1);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult.Value);
            Assert.Equal(_articleDTOs.First(), model);
        }

        [Fact]
        public async void GetArticleInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);
            var result = await controller.GetArticle(40);

            Assert.IsType<NotFoundResult>(result);

            var result2 = await controller.GetArticle(-1);
            Assert.IsType<BadRequestResult>(result2);
        }

        [Fact]
        public async void GetArticlesTest()
        {

            var controller = new ArticlesController(_context, _userManager);
            var result = await controller.GetArticles(1, null);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleListDTO>(objectResult.Value);
            
            Assert.Equal(_articleListDTO, model);
        }

        [Fact]
        public async void GetArticlesOverPageTest()
        {

            var controller = new ArticlesController(_context, _userManager);
            var result = await controller.GetArticles(2, null);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleListDTO>(objectResult.Value);

            var emptyDTO = new ArticleListDTO()
            {
                Limit = 20,
                Count = _articleDTOs.Count,
                PageCount = 1,
                Page = 2,
                Articles = new List<ArticleListElemDTO>()
            };

            Assert.Equal(emptyDTO, model);
        }


        [Fact]
        public async void PostArticleTest()
        {
            var controller = new ArticlesController(_context, _userManager);
            var article = _articleDTOs.First();
            article.Name = "Changed test name";
            article.Lead = "Changed up the lead as well";
            article.Images.RemoveAt(0);

            var result = await controller.PostArticle(article.Id, article);

            // Assert
            var objectResult = Assert.IsType<NoContentResult>(result);

            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);            

            Assert.Equal(article, model);
        }


        [Fact]
        public async void PostArticleEmptyFieldTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            //POST TEST 1 (Lead empty)
            var article = new ArticleDTO(_articleDTOs.First());            
            article.Lead = "";

            var result = await controller.PostArticle(article.Id, article);            
            var objectResult = Assert.IsType<BadRequestResult>(result);

            //POST TEST 2 (Name empty)
            var article2 = new ArticleDTO(_articleDTOs.First());
            article.Name = "";

            var result2 = await controller.PostArticle(article.Id, article);
            var objectResult2 = Assert.IsType<BadRequestResult>(result2);

            //POST TEST 3 (Content empty)
            var article3 = new ArticleDTO(_articleDTOs.First());
            article.Content = "";

            var result3 = await controller.PostArticle(article.Id, article);
            var objectResult3 = Assert.IsType<BadRequestResult>(result3);


            //GET TEST (Article unchanged)
            var result4 = await controller.GetArticle(article.Id);

            var objectResult4 = Assert.IsType<OkObjectResult>(result4);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult4.Value);

            Assert.Equal(_articleDTOs.First(), model);
        }


        [Fact]
        public async void PostArticleInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = new ArticleDTO(_articleDTOs.First());
            int originalId = article.Id;
            article.Lead = "Change";

            //Id does not match article Id
            var result = await controller.PostArticle(article.Id+1, article);
            Assert.IsType<BadRequestResult>(result);


            //Id matches but does not exist
            article.Id = 40;

            var result2 = await controller.PostArticle(article.Id, article);
            Assert.IsType<NotFoundResult>(result2);


            //GET TEST (Article unchanged)
            var result3 = await controller.GetArticle(originalId);

            var objectResult3 = Assert.IsType<OkObjectResult>(result3);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult3.Value);

            Assert.Equal(_articleDTOs.First(), model);
        }

        [Fact]
        public async void PublishArticleTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsPublished == false);

            Assert.True(article.IsPublished == false);
            Assert.True(article.PublishedAt == null);

            //POST TEST (Lead empty)
            var result = await controller.PostPublishArticle(article.Id);

            var objectResult = Assert.IsType<OkResult>(result);

            //GET TEST (Article published)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(model.IsPublished == true);
            Assert.True(model.PublishedAt != null);
        }


        [Fact]
        public async void PublishArticleInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var result = await controller.PostPublishArticle(40);

            Assert.IsType<NotFoundResult>(result);

            var result2 = await controller.PostPublishArticle(-1);
            Assert.IsType<BadRequestResult>(result2);
        }

        [Fact]
        public async void PublishArticleAlreadyPublishedTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsPublished == true);

            Assert.True(article.IsPublished == true);
            Assert.True(article.PublishedAt != null);

            //POST TEST (Lead empty)
            var result = await controller.PostPublishArticle(article.Id);

            var objectResult = Assert.IsType<OkResult>(result);

            //GET TEST (Article published)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(model.IsPublished == true);
            Assert.True(model.PublishedAt != null);
        }

        [Fact]
        public async void UnPublishArticleTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsPublished == true);

            Assert.True(article.IsPublished == true);
            Assert.True(article.PublishedAt != null);

            //POST TEST (Lead empty)
            var result = await controller.PostUnPublishArticle(article.Id);

            var objectResult = Assert.IsType<OkResult>(result);

            //GET TEST (Article published)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(model.IsPublished == false);
            Assert.True(model.PublishedAt == null);
        }

        [Fact]
        public async void UnPublishArticleInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);            
           
            var result = await controller.PostUnPublishArticle(40);
            Assert.IsType<NotFoundResult>(result);

            var result2 = await controller.GetArticle(-1);

            Assert.IsType<BadRequestResult>(result2);
        }

        [Fact]
        public async void UnPublishArticleAlreadyUnPublishedTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsPublished == false);

            Assert.True(article.IsPublished == false);
            Assert.True(article.PublishedAt == null);
            
            //POST TEST (Unpublish)
            var result = await controller.PostUnPublishArticle(article.Id);
            var objectResult = Assert.IsType<OkResult>(result);


            //GET TEST (Article still unpublished)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(model.IsPublished == false);
            Assert.True(model.PublishedAt == null);
        }

        [Fact]
        public async void HighlightArticleTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsHighlighted == false && a.IsPublished == true);
            var highlightedArticle = _articleDTOs.First(a => a.IsHighlighted == true);

            Assert.True(article.IsHighlighted == false);
            Assert.True(article.HighlightedAt == null);

            Assert.True(article.IsPublished == true);
            Assert.True(article.PublishedAt != null);

            Assert.True(highlightedArticle.IsHighlighted == true);
            Assert.True(highlightedArticle.HighlightedAt != null);

            var result = await controller.PostHighlightArticle(article.Id);
            var objectResult = Assert.IsType<OkResult>(result);


            //GET TEST (Article highlighted)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model2 = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(model2.IsHighlighted == true);
            Assert.True(model2.HighlightedAt != null);

            
            
            //GET TEST 2 (Highlighted article no longer highlighted)
            var result3 = await controller.GetArticle(highlightedArticle.Id);

            var objectResult3 = Assert.IsType<OkObjectResult>(result3);
            var model3 = Assert.IsAssignableFrom<ArticleDTO>(objectResult3.Value);
            Assert.True(model3.IsHighlighted == false);
            Assert.True(model3.HighlightedAt == null);
        }


        [Fact]
        public async void HighlightArticleInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var result = await controller.PostHighlightArticle(40);

            Assert.IsType<NotFoundResult>(result);

            var result2 = await controller.PostHighlightArticle(-1);
            Assert.IsType<BadRequestResult>(result2);
        }

        [Fact]
        public async void HighlightArticleAlreadyHighlightedTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsHighlighted == true);

            Assert.True(article.IsHighlighted == true);
            Assert.True(article.HighlightedAt != null);

            //POST TEST (Highlight)
            var result = await controller.PostHighlightArticle(article.Id);
            var objectResult = Assert.IsType<OkResult>(result);


            //GET TEST (Article still highlighted)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(article.IsHighlighted == true);
            Assert.True(article.HighlightedAt != null);
        }

        [Fact]
        public async void HighlightArticleNotPublishedTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsHighlighted == false && a.IsPublished == false);

            Assert.True(article.IsHighlighted == false);
            Assert.True(article.HighlightedAt == null);
            Assert.True(article.IsPublished == false);
            Assert.True(article.PublishedAt == null);

            //POST TEST (Highlight)
            var result = await controller.PostHighlightArticle(article.Id);
            Assert.IsType<BadRequestResult>(result);

            //GET TEST (Article unchanged)
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.True(article.IsHighlighted == false);
            Assert.True(article.HighlightedAt == null);
        }


        [Fact]
        public async void PutImageTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsHighlighted == true);

            ImageDTO newImage = new ImageDTO()
            {
                Name = "New Image",
                Base64 = article.Images.First().Base64
            }
            ;

            //POST TEST (Highlight)
            var result = await controller.PutImage(article.Id, newImage);
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ImageDTO>(objectResult.Value);

            newImage.Id = model.Id;
            Assert.Equal(newImage, model);


            //GET TEST (Image added to Article)
            article.Images.Add(newImage);
            var result2 = await controller.GetArticle(article.Id);

            var objectResult2 = Assert.IsType<OkObjectResult>(result2);
            var model2 = Assert.IsAssignableFrom<ArticleDTO>(objectResult2.Value);
            Assert.Equal(article, model2);
        }


        [Fact]
        public async void PutImageInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsHighlighted == true);

            ImageDTO newImage = new ImageDTO()
            {
                Name = "New Image",
                Base64 = article.Images.First().Base64
            }
            ;

            
            var result = await controller.PutImage(40, newImage);
            Assert.IsType<NotFoundResult>(result);


            var result2 = await controller.PutImage(-1, newImage);
            Assert.IsType<BadRequestResult>(result2);
        }



        [Fact]
        public async void PutImageEmptyDataTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First(a => a.IsHighlighted == true);

            ImageDTO newImage = new ImageDTO()
            {
                Name = "New Image",
                Base64 = ""
            }
            ;


            var result = await controller.PutImage(article.Id, newImage);
            Assert.IsType<BadRequestResult>(result);
        }



        [Fact]
        public async void DeleteArticleTest()
        {
            var controller = new ArticlesController(_context, _userManager);

            var article = _articleDTOs.First();


            var result = await controller.DeleteArticle(article.Id);
            Assert.IsType<OkResult>(result);

            var result2 = await controller.GetArticle(article.Id);
            Assert.IsType<NotFoundResult>(result2);

        }



        [Fact]
        public async void DeleteArticleInvalidIdTest()
        {
            var controller = new ArticlesController(_context, _userManager);
            
            var result = await controller.DeleteArticle(40);
            Assert.IsType<NotFoundResult>(result);

            var result2 = await controller.GetArticle(-1);
            Assert.IsType<BadRequestResult>(result2);

        }

    }



    public class FakeSignInManager : SignInManager<Editor>
    {
        public FakeSignInManager(FakeUserManager usermanager)
                : base(usermanager,
                     new Mock<IHttpContextAccessor>().Object,
                     new Mock<IUserClaimsPrincipalFactory<Editor>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<ILogger<SignInManager<Editor>>>().Object,
                     new Mock<IAuthenticationSchemeProvider>().Object)
        { }
    }



    public class FakeUserManager : UserManager<Editor>
    {
        public FakeUserManager(NewsContext context)
            : base(new Mock<IUserPasswordStore<Editor>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<Editor>>().Object,
              new IUserValidator<Editor>[0],
              new IPasswordValidator<Editor>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<Editor>>>().Object)
        {
            _editorList = new List<Editor>();
            _context = context;
        }

        private List<Editor> _editorList;
        private NewsContext _context;

        public override Task<IdentityResult> CreateAsync(Editor user, string password)
        {
            _editorList.Add(user);
            _context.Users.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<Editor> GetUserAsync(System.Security.Claims.ClaimsPrincipal User)
        {
            return Task.FromResult(_editorList.FirstOrDefault());
        }

        public override Task<IdentityResult> AddToRoleAsync(Editor user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(Editor user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}
