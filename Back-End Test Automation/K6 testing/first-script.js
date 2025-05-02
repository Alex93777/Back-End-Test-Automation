import http from 'k6/http'
import { sleep } from 'k6'

export const options = {                            //колко потребители ще правят това действие и за колко дълго време
    ext: {
        loadimpact: {
            projectID: 3727570
        }
    },
    vus: 10,
    duration: '10s'
}


export default function(){                          //действието който нашите потребители ще правят
    http.get('https://test.k6.io/')
    sleep(1)
}


