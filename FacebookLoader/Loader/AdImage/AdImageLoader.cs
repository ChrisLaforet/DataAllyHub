﻿using System;
using System.Collections.Generic;
using FacebookLoader.Common;
using FacebookLoader.Content;
using Newtonsoft.Json.Linq;

namespace FacebookLoader.Loader.AdImage;

public class FacebookAdImagesLoader : FacebookLoaderBase
{
    private const string FIELDS_LIST = "account_id,created_time,creatives,hash,id,is_associated_creatives_in_adgroups," +
                                       "name,permalink_url,status,updated_time,url,url_128";

    private const int LIMIT = 500;
    private const int MAX_TEST_LOOPS = 4;
    
    public FacebookAdImagesLoader(FacebookParameters facebookParameters) : base(facebookParameters) {}

    public FacebookAdImagesResponse StartLoad(bool testMode = false)
    {
        string url = $"{FacebookParameters.CreateUrlFor("adimages")}?fields={FIELDS_LIST}&limit={LIMIT}&access_token={FacebookParameters.Token}";
        return Load(url, testMode);
    }

    public async FacebookAdImagesResponse Load(string startUrl, bool testMode = false)
    {
        int loopCount = 0;
        string currentUrl = startUrl;
        var records = new List<FacebookAdImage>();

        while (true)
        {
            try
            {
                var data = await CallGraphApiAsync(currentUrl);
                var root = Root.FromJson(data);

                foreach (var item in root.Data)
                {
                    var image = new FacebookAdImage(
                        item.Id,
                        item.Name,
                        item.AccountId,
                        new List<string>(),
                        item.Hash,
                        item.IsAssociatedCreativesInAdgroups,
                        item.PermalinkUrl,
                        item.Status,
                        item.CreatedTime,
                        item.UpdatedTime,
                        item.Url,
                        item.Url128
                    );

                    records.Add(image);
                }

                if (string.IsNullOrEmpty(root.Paging.Next) || (testMode && loopCount >= MAX_TEST_LOOPS))
                    break;

                currentUrl = root.Paging.Next;
                loopCount++;
            }
            catch (FacebookHttpException fe)
            {
                Console.WriteLine($"Caught FacebookHttpException at FacebookAdImagesLoader.Load(): {fe.Message}");
                return new FacebookAdImagesResponse(records, false, currentUrl, fe.NotPermitted, fe.TokenExpired, fe.Throttled);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception at FacebookAdImagesLoader.Load(): {ex.Message}");
                return new FacebookAdImagesResponse(records, false, currentUrl, true);
            }
        }

        return new FacebookAdImagesResponse(records);
    }
}

class Cursors
{
    public string Before { get; set; }
    public string After { get; set; }

    public static Cursors FromJson(JObject obj)
    {
        return new Cursors
        {
            Before = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "before"),
            After = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "after")
        };
    }
}

class Content
{
    public string AccountId { get; set; }
    public string CreatedTime { get; set; }
    public List<string> Creatives { get; set; }
    public string Hash { get; set; }
    public string Id { get; set; }
    public bool IsAssociatedCreativesInAdgroups { get; set; }
    public string Name { get; set; }
    public string PermalinkUrl { get; set; }
    public string Status { get; set; }
    public string UpdatedTime { get; set; }
    public string Url { get; set; }
    public string Url128 { get; set; }

    public static Content FromJson(JObject obj)
    {
        return new Content
        {
            AccountId = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "account_id"),
            CreatedTime = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "created_time"),
            Creatives = FacebookLoader.Content.FacebookLoaderBase.ExtractObjectArray(obj, "creatives"),
            Hash = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "hash"),
            Id = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "id"),
            IsAssociatedCreativesInAdgroups = FacebookLoader.Content.FacebookLoaderBase.ExtractBoolean(obj, "is_associated_creatives_in_adgroups"),
            Name = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "name"),
            PermalinkUrl = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "permalink_url"),
            Status = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "status"),
            UpdatedTime = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "updated_time"),
            Url = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "url"),
            Url128 = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "url_128")
        };
    }
}

class Paging
{
    public Cursors Cursors { get; set; }
    public string Next { get; set; }

    public static Paging FromJson(JObject obj)
    {
        return new Paging
        {
            Cursors = Cursors.FromJson((JObject)FacebookLoader.Content.FacebookLoaderBase.ExtractObject(obj, "cursors")),
            Next = FacebookLoader.Content.FacebookLoaderBase.ExtractString(obj, "next")
        };
    }
}

class Root
{
    public List<Content> Data { get; set; }
    public Paging Paging { get; set; }

    public static Root FromJson(JObject obj)
    {
        return new Root
        {
            Data = FacebookLoader.Content.FacebookLoaderBase.ExtractObjectArray(obj, "data", Content.FromJson),
            Paging = Paging.FromJson((JObject) FacebookLoader.Content.FacebookLoaderBase.ExtractObject(obj, "paging"))
        };
    }
}




// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using Newtonsoft.Json;
//
// public class FacebookAdImagesLoader : FacebookLoaderBase
// {
//     private const string FIELDS_LIST = "account_id,created_time,creatives,hash,id,is_associated_creatives_in_adgroups,name,permalink_url,status,updated_time,url,url_128";
//     private const int LIMIT = 500;
//     private const int MAX_TEST_LOOPS = 4;
//
//     public FacebookAdImagesLoader(FacebookParameters facebookParameters) : base(facebookParameters) { }
//
//     public FacebookAdImagesResponse StartLoad(bool testMode = false)
//     {
//         string url = $"{facebookParameters.CreateUrlFor("adimages")}?fields={FIELDS_LIST}&limit={LIMIT}&access_token={facebookParameters.Token}";
//         return Load(url, testMode);
//     }
//
//     public FacebookAdImagesResponse Load(string startUrl, bool testMode = false)
//     {
//         int loopCount = 0;
//         string currentUrl = startUrl;
//         List<FacebookAdImage> records = new List<FacebookAdImage>();
//
//         while (true)
//         {
//             try
//             {
//                 var data = FacebookLoaderBase.CallGraphApi(currentUrl);
//                 var root = JsonConvert.DeserializeObject<Root>(data);
//                 
//                 foreach (var item in root.Data)
//                 {
//                     records.Add(new FacebookAdImage
//                     {
//                         Id = item.Id,
//                         Name = item.Name,
//                         AccountId = item.AccountId,
//                         Creatives = new List<Creative>(),
//                         Hash = item.Hash,
//                         IsAssociatedCreativesInAdgroups = item.IsAssociatedCreativesInAdgroups,
//                         PermalinkUrl = item.PermalinkUrl,
//                         Status = item.Status,
//                         CreatedTime = item.CreatedTime,
//                         UpdatedTime = item.UpdatedTime,
//                         Url = item.Url,
//                         Url128 = item.Url128
//                     });
//                 }
//
//                 if (string.IsNullOrEmpty(root.Paging.Next) || (testMode && loopCount >= MAX_TEST_LOOPS))
//                 {
//                     break;
//                 }
//
//                 currentUrl = root.Paging.Next;
//                 loopCount++;
//             }
//             catch (FacebookHttpException fe)
//             {
//                 Console.WriteLine($"Caught FacebookHttpException at FacebookAdImagesLoader.Load(): {fe}");
//                 return new FacebookAdImagesResponse(records, false, currentUrl, fe.NotPermitted, fe.TokenExpired, fe.Throttled);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Caught exception at FacebookAdImagesLoader.Load(): {ex}");
//                 return new FacebookAdImagesResponse(records, false, currentUrl, true);
//             }
//         }
//
//         return new FacebookAdImagesResponse(records);
//     }
// }
//
//
// public class Cursors
// {
//     public string Before { get; set; }
//     public string After { get; set; }
//
//     public static Cursors FromDict(Dictionary<string, object> obj)
//     {
//         var _before = FacebookLoaderBase.ExtractString(obj, "before");
//         var _after = FacebookLoaderBase.ExtractString(obj, "after");
//         return new Cursors { Before = _before, After = _after };
//     }
// }
//
// public class Content
// {
//     public string AccountId { get; set; }
//     public string CreatedTime { get; set; }
//     public List<string> Creatives { get; set; }
//     public string Hash { get; set; }
//     public string Id { get; set; }
//     public bool IsAssociatedCreativesInAdgroups { get; set; }
//     public string Name { get; set; }
//     public string PermalinkUrl { get; set; }
//     public string Status { get; set; }
//     public string UpdatedTime { get; set; }
//     public string Url { get; set; }
//     public string Url128 { get; set; }
//
//     public static Content FromDict(Dictionary<string, object> obj)
//     {
//         var _accountId = FacebookLoaderBase.ExtractString(obj, "account_id");
//         var _createdTime = FacebookLoaderBase.ExtractString(obj, "created_time");
//         var _creatives = FacebookLoaderBase.ExtractObjectArray(obj, "creatives");
//         var _hash = FacebookLoaderBase.ExtractString(obj, "hash");
//         var _id = FacebookLoaderBase.ExtractString(obj, "id");
//         var _isAssociatedCreativesInAdgroups = FacebookLoaderBase.ExtractBoolean(obj, "is_associated_creatives_in_adgroups");
//         var _name = FacebookLoaderBase.ExtractString(obj, "name");
//         var _permalinkUrl = FacebookLoaderBase.ExtractString(obj, "permalink_url");
//         var _status = FacebookLoaderBase.ExtractString(obj, "status");
//         var _updatedTime = FacebookLoaderBase.ExtractString(obj, "updated_time");
//         var _url = FacebookLoaderBase.ExtractString(obj, "url");
//         var _url128 = FacebookLoaderBase.ExtractString(obj, "url_128");
//         return new Content
//         {
//             AccountId = _accountId,
//             CreatedTime = _createdTime,
//             Creatives = _creatives,
//             Hash = _hash,
//             Id = _id,
//             IsAssociatedCreativesInAdgroups = _isAssociatedCreativesInAdgroups,
//             Name = _name,
//             PermalinkUrl = _permalinkUrl,
//             Status = _status,
//             UpdatedTime = _updatedTime,
//             Url = _url,
//             Url128 = _url128
//         };
//     }
// }
//
// public class Paging
// {
//     public Cursors Cursors { get; set; }
//     public string Next { get; set; }
//
//     public static Paging FromDict(Dictionary<string, object> obj)
//     {
//         var _cursors = Cursors.FromDict(FacebookLoaderBase.ExtractObject(obj, "cursors"));
//         var nextValue = FacebookLoaderBase.ExtractObject(obj, "next");
//         var _next = nextValue != null ? nextValue.ToString() : null;
//         return new Paging { Cursors = _cursors, Next = _next };
//     }
// }
//
// public class Root
// {
//     public List<Content> Data { get; set; }
//     public Paging Paging { get; set; }
//
//     public static Root FromDict(Dictionary<string, object> obj)
//     {
//         var _data = new List<Content>();
//         foreach (var item in FacebookLoaderBase.ExtractObjectArray(obj, "data"))
//         {
//             _data.Add(Content.FromDict(item));
//         }
//         var _paging = Paging.FromDict(FacebookLoaderBase.ExtractObject(obj, "paging"));
//         return new Root { Data = _data, Paging = _paging };
//     }
// }