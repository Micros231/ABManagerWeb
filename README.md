# ABManagerWeb

# RESTAPIs:
Request -> Responce

1)GetCurrentVersion (Method: GET)
URL/versions/current -> {version:string}

2)GetManifestInfoByVersion (Method: GET)
URL/content/{version:string}/manifest/info -> {jsonManifestInfo}

3)GetCurrentManifestInfo (Method: GET)
URL/content/manifest/info -> {jsonManifestInfo}

4)DownloadManifestFileByVersion (Method: GET)
URL/content/{version:string}/manifest -> {jsonFile|application/json}

5)DownloadCurrentManifestFile (Method: GET)
URL/content/manifest -> {jsonFile|application/json}

6)UploadManifest (Method: POST|application/json)
URL/contnet/uploadManifest -> OK
