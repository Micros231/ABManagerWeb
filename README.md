# ABManagerWeb

# RESTAPIs:
Request -> Responce

1)GetCurrentVersion (Method: GET)
URL/versions/current -> {version:string}

2)GetManifestInfoByVersion (Method: GET)
URL/{version:string}/manifest/info -> {jsonManifestInfo}

3)GetCurrentManifestInfo (Method: GET)
URL/manifest/info -> {jsonManifestInfo}

4)DownloadManifestFileByVersion (Method: GET)
URL/{version:string}/manifest -> {jsonFile|application/json}

5)DoawnloadCurrentManifestFile (Method: GET)
URL/manifest -> {jsonFile|application/json}

6)UploadManifest (Method: POST|application/json)
URL/uloadmanifest -> OK
