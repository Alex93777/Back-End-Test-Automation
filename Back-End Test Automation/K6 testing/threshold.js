import http from 'k6/http'
import { sleep } from 'k6'
import  { check } from 'k6'

export const options = {
    vus: 10,
    duration: '30s',
    thresholds: {
        'http_req_duration': ['p(95)<50', 'p(90)<25'],
        'http_req_failed': ['rate<0.01'],
    } 
}

export default function(){
    const response = http.get('https://test.k6.io')

    check(response, {
        'HTTP status is 200': (r) => r.status === 200,
        'Collection of simple web-pages suitable for load testing. is displayed': (r) => r.body.
        includes('Collection of simple web-pages suitable for load testing.'), 
    })
    sleep(1);
}