import * as React from 'react';
import httpClient from 'axios';
import {connect} from 'react-redux';
import {CurrencyDto} from "./CurrencyDto";

function isLoading() {
    return (
        <p>Cargando ...</p>
    )
}

class Home extends React.PureComponent {

    state = {
        busy: true,
        currencyDto: new CurrencyDto()
    }

    async componentDidMount() {
        const response = await httpClient.get<CurrencyDto>(`Currency`);
        this.setState({busy: false, currencyDto: response.data});
    }

    isBusy(): boolean {
        return this.state.busy
    }

    render() {
        if (this.isBusy()) {
            return isLoading();
        }
        const currencyDto = this.state.currencyDto;
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
        )
    }
}

export default connect()(Home);
