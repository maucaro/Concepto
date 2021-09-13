$FILES_PATH = $args[0]
$AUDIENCE = "test-audience"
$EMAIL = "test@maucaro.com"
$SUB = "xyz"
$TENANT = "maucaro"
$PYTHON_FILE = $FILES_PATH+"\gen-jwt.py"
$PEM_FILE = $FILES_PATH+"\key.pem"

#Good token
python.exe $PYTHON_FILE -iss foo.bar -aud $AUDIENCE -claims=email:$EMAIL,sub:$SUB -nestedclaim firebase tenant $TENANT -expire 100000 $PEM_FILE > $FILES_PATH\token

#Wrong audience
python.exe $PYTHON_FILE -iss foo.bar -aud wrongaud -claims=email:$EMAIL,sub:$SUB -nestedclaim firebase tenant $TENANT -expire 100000 $PEM_FILE > $FILES_PATH\token_wrong_aud

#Wrong tenant
python.exe $PYTHON_FILE -iss foo.bar -aud $AUDIENCE -claims=email:$EMAIL,sub:$SUB -nestedclaim firebase tenant wrongtenant -expire 100000 $PEM_FILE > $FILES_PATH\token_wrong_tenant

#Expired token
python.exe $PYTHON_FILE -iss foo.bar -aud $AUDIENCE -claims=email:$EMAIL,sub:$SUB -nestedclaim firebase tenant $TENANT -expire -100000 $PEM_FILE > $FILES_PATH\token_expired

#Wrong Key
python.exe $PYTHON_FILE -iss foo.bar -aud $AUDIENCE -claims=email:$EMAIL,sub:$SUB -nestedclaim firebase tenant $TENANT -expire 100000 $FILES_PATH\wrongkey.pem > $FILES_PATH\token_wrong_key

#No email
python.exe $PYTHON_FILE -iss foo.bar -aud $AUDIENCE -claims=sub:$SUB -nestedclaim firebase tenant $TENANT -expire 100000 $PEM_FILE > $FILES_PATH\token_no_email

#No tenant
python.exe $PYTHON_FILE -iss foo.bar -aud $AUDIENCE -claims=email:$EMAIL,sub:$SUB -expire 100000 $PEM_FILE > $FILES_PATH\token_no_tenant


# SIG # Begin signature block
# MIIFlAYJKoZIhvcNAQcCoIIFhTCCBYECAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQU6hjYcbAeEF6z6GNwnps8pWQN
# e2GgggMiMIIDHjCCAgagAwIBAgIQMXRH8MlsjpdCj6lY0o91jjANBgkqhkiG9w0B
# AQsFADAnMSUwIwYDVQQDDBxQb3dlclNoZWxsIENvZGUgU2lnbmluZyBDZXJ0MB4X
# DTIxMDkxMjEyMDQwN1oXDTIyMDkxMjEyMjQwN1owJzElMCMGA1UEAwwcUG93ZXJT
# aGVsbCBDb2RlIFNpZ25pbmcgQ2VydDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCC
# AQoCggEBAMrF2zmyP2zFv9q9EQXvh/t2qFESn6qCOdwOu6lVCGAz8nbXFoL2WZ6O
# tr3J4Ohh6gozRToDwwdC5vvrcYK3x9lke5uFGIdb2cn3ooB82ttktnWlYNQmKnyU
# nzyK4bZ/uXodpo7zJUkVHqY0Vvz2kHPMwhj34AyM1CEtTm3nViBEM+bdRwV7j8Zp
# FYTjVzYuVwMotaNyZMZfYWHBoUUGNWTCEiKZcbpA0QLZdV1beb/YB44tU7+qCitG
# YFqTJv9yCVXHTRSG0oxqxjx5TLxYWJnWZOlxjieI6l8JeaMLcx8TNOm36m/kePju
# FSKJ2SjYy9GTvxA4CF3UjzgJmthZUg0CAwEAAaNGMEQwDgYDVR0PAQH/BAQDAgeA
# MBMGA1UdJQQMMAoGCCsGAQUFBwMDMB0GA1UdDgQWBBRdB9KqCfFEYObHi1yG2ebF
# ER2fGzANBgkqhkiG9w0BAQsFAAOCAQEAo8fqmx9/3XSL9hWv8rVty2W1Mg3S/TG8
# gN0qwxWXBZpHWaF3m4jb9cz5N0LO272rJmxy0zYLB/aR1a1YTulbm/b2YA0Q40lv
# XoEC9YkR4TMZHEK9jt+rU6Zemt+yQJP0laWj3V2AewZHHcmiu5pX3O8bpRlWoVQL
# z2+hi6hiPG2YIqc9bqVVBTGOeqid5bAbHiIGlNUwOPMgyL8DCKlA6nNUVleT/bX8
# 1a1DIIW3QKCfdbxAbSDLni/VcWT5DE22Kf+4Y/C+Ia9Dcj2GJwlCI9Ao3YO5ugcj
# xsy1gGM0OMMDEel2cYJMbxbzosyfJftzruk/e149N4bNg/pMHRfDIzGCAdwwggHY
# AgEBMDswJzElMCMGA1UEAwwcUG93ZXJTaGVsbCBDb2RlIFNpZ25pbmcgQ2VydAIQ
# MXRH8MlsjpdCj6lY0o91jjAJBgUrDgMCGgUAoHgwGAYKKwYBBAGCNwIBDDEKMAig
# AoAAoQKAADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgEL
# MQ4wDAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQUFoMlAi+W9HaHLmXGayEf
# 4VQde8MwDQYJKoZIhvcNAQEBBQAEggEAbcVBSkD86bjtYLyjqHhQUHChZx2Jzyjk
# DYOpudh0ebtuaeGrp1IJ61nq3JT19LWpA6UcUVzIwim/YyzGBdB4ShIgLqZ5DAXc
# J7mNw4DogVyenhnkbqqkl+SmFqwugru0xSIu/Dm5rrvCeL3XkAJH1YMiuKcPw36I
# 4+HDfXTzn08L//TT3524GhRuPDsAa8nwAIEe9+OqLUwS2NIKKett4+xJo6nUCvQU
# 67y1AEh94nL8seiYAp5aKcD3OizRRewPmRZ+9qLb2Z9G5PAGHhOUiplqv0ppQtEd
# Vrl1qV94KQcyKXLQJG4gr6+7q0X/9TVa5lZeJTsX6s7RVqpZoEeK2g==
# SIG # End signature block
