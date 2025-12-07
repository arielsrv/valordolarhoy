import React, { useEffect, useState } from 'react';
import httpClient from 'axios';
import { CurrencyDto } from "./CurrencyDto";

function Home() {
    const [busy, setBusy] = useState(true);
    const [currencyDto, setCurrencyDto] = useState<CurrencyDto>(new CurrencyDto());

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await httpClient.get<CurrencyDto>(`/api/Currency`);
                setCurrencyDto(response.data);
                setBusy(false);
            } catch (error) {
                console.error('Error fetching currency data:', error);
                setBusy(false);
            }
        };
        
        fetchData();
    }, []);

    if (busy) {
        return <p>Cargando ...</p>;
    }

    return (
        <div>
            <p>Cotización oficial y blue</p>
            <ul>
                <li>Oficial: Compra: ARS {currencyDto.official.buy}$ |
                    Venta: ARS {currencyDto.official.sell}$
                </li>
                <li>Blue: Compra: ARS {currencyDto.blue.buy}$ |
                    Venta: ARS {currencyDto.blue.sell}$
                </li>
            </ul>
        </div>
    );
}

export default Home;

