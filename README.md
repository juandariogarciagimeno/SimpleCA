```
 _____ _                 _      _____       
/  ___(_)               | |    /  __ \      
\ `--. _ _ __ ___  _ __ | | ___| /  \/ __ _ 
 `--. \ | '_ ` _ \| '_ \| |/ _ \ |    / _` |
/\__/ / | | | | | | |_) | |  __/ \__/\ (_| |
\____/|_|_| |_| |_| .__/|_|\___|\____/\__,_|
                  | |                       
                  |_|
```

---
A simple Certificate Authority (CA) for mock and testing purposes

## Features

- [x] Certificate Generation
  - [x] crt and pfx in a zip file
  - [x] crt and pfx in a json
- [x] Certificate Revocation
  - [x] Posting a .crt
  - [x] Posting a json containing the public key
  - [x] OCSP Verification

---
## Configuration

The CA subject is customized through environment variables. An example is provided in the repo `.env` file

```env
CA_CN=CATEST
CA_O=TEST
CA_OU=TEST
CA_S=Madrid
CA_L=Madrid
CA_C=ES
CA_E=testca@test.com
CA_PWD=Abc123
CRL_HOSTS=http://localhost:8008;https://my.custom.domain
```

Note.

`CRL_HOST` must contain a `;` separated list of possible hosts the CA is going to be deployed. If a custom domain name or port is used must be included.

## Deployment

Through docker compose

```yml
services:
    simpleca:
        image: juandariogg/simpleca:latest
        ports:
            - 8008:8008
        env_file: ".env"
        network_mode: host
```