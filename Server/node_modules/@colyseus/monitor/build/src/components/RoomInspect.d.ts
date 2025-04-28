import * as React from "react";
import 'react18-json-view/src/style.css';
interface Props {
}
interface State {
    roomId?: string;
    state: any;
    clients: Array<{
        sessionId: string;
        elapsedTime: number;
    }>;
    maxClients: number;
    stateSize: number;
    locked: boolean;
    currentTab: string;
    sendDialogTitle: string;
    sendDialogOpen: boolean;
    sendToClient?: any;
    sendType: string;
    sendData: string;
}
export declare class RoomInspect extends React.Component<Props, State> {
    state: State;
    updateDataInterval: number;
    componentDidMount(): void;
    fetchRoomData(): void;
    roomCall(method: string, ...args: any[]): Promise<void>;
    componentWillUnmount(): void;
    sendMessage(sessionId?: string): void;
    disconnectClient(sessionId: string): void;
    disposeRoom(): void;
    updateSendType: (e: any) => void;
    updateSendData: (changes: any) => void;
    handleCloseSend: () => void;
    handleSend: () => void;
    goBack(): void;
    handleTabChange: (event: React.SyntheticEvent, newValue: string) => void;
    render(): React.JSX.Element;
}
export {};
