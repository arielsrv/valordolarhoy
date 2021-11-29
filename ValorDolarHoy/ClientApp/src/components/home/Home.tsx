import * as React from 'react';
import httpClient from 'axios';
import { connect } from 'react-redux';
import { CurrencyDto } from "./CurrencyDto";

function isLoading() {
    return (
        <p>Cargando ...</p>
    )
}

class Home extends React.PureComponent {

    state = {
        busy: true,
        bluelyticsDto: new CurrencyDto()
    }

    async componentDidMount() {
        const response = await httpClient.get<CurrencyDto>(`Currency`);
        this.setState({busy: false, bluelyticsDto: response.data});
    }

    isBusy(): boolean {
        return this.state.busy
    }

    render() {
        if (this.isBusy()) {
            return isLoading();
        }
        const bluelyticsDto = this.state.bluelyticsDto;
        return (
            <div>
                <p>Cotización oficial y blue</p>
                <ul>
                    <li>Oficial: Compra: ARS {bluelyticsDto.official.buy}$ |
                        Venta: ARS {bluelyticsDto.official.sell}$
                    </li>
                    <li>Blue: Compra: ARS {bluelyticsDto.blue.buy}$ |
                        Venta: ARS {bluelyticsDto.blue.sell}$
                    </li>
                </ul>
            </div>
        )
    }
}

export default connect()(Home);
